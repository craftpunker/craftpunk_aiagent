
#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

public enum AudioConf
{
    Normal = 0,
    Soldier = 1,
    Skill = 2,
}

public static class AudioType
{
    public static string BornAudio = "bornAudio";
    public static string MoveAudio = "moveAudio";
    public static string AttackAudio = "attackAudio";
    public static string DeadAudio = "deadAudio";
}

public class AudioMgr : MonoSingleton<AudioMgr>
{
    private const int AUDIO_CHANNEL_NUM = 9;    // ����Դ����
    //private const float intervalTime = 0.3f; //ͬ��Դ���ż��ʱ��
    //��key : (audioName �� AudioClip)
    private Dictionary<string, Dictionary<string, AudioClip>> audioClipOneShotDict = new Dictionary<string, Dictionary<string, AudioClip>>();
    private Dictionary<string, AudioClip> audioClipLoopDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> audioClipNoGroupOneShotDict = new Dictionary<string, AudioClip>();
    private GameObject Sound;
    private GameObject BGM;

    private bool IsBgmOn;
    private bool IsSoundOn;

    private Dictionary<string, List<string>> audioNameDict = new Dictionary<string, List<string>>();

    private struct CHANNEL
    {
        public AudioSource channel;
        public string key; //����
        public float intervalTime; //ͬ����Դ���ż��ʱ��
        public float keyOnTime; //��¼���һ�β������ֵ�ʱ��
        public int audioType; //��¼��Ч����
    };
    private CHANNEL[] m_channels;

    private void Start()
    {
        Sound = transform.Find("Sound").gameObject;

        m_channels = new CHANNEL[AUDIO_CHANNEL_NUM];
        for (int i = 0; i < AUDIO_CHANNEL_NUM; i++)
        {
            //ÿ��Ƶ����Ӧһ����Դ
            m_channels[i].channel = Sound.AddComponent<AudioSource>();
            m_channels[i].keyOnTime = 0;
            m_channels[i].intervalTime = 0;
        }

        if (!PlayerPrefs.HasKey("isOnBGM"))
        {
            PlayerPrefs.SetInt("isOnBGM", 1);
        }

        if (!PlayerPrefs.HasKey("isOnSound"))
        {
            PlayerPrefs.SetInt("isOnSound", 1);
        }

        IsBgmOn = PlayerPrefs.GetInt("isOnBGM") == 0 ? false : true;
        IsSoundOn = PlayerPrefs.GetInt("isOnSound") == 0 ? false : true;

        SwitchBgmVolume(IsBgmOn);
        SwitchSoundVolume(IsSoundOn);
        //StartCoroutine(Test());
    }

    //IEnumerator Test()
    //{
    //    while (true)
    //    {
    //        AudioMgr.instance.PlayOneShot(AudioConf.Soldier, "10101", AudioType.AttackAudio);
    //        AudioMgr.instance.PlayOneShot(AudioConf.Soldier, "10101", AudioType.DeadAudio);
    //        yield return new WaitForUpdate();
    //    }
    //}

    private string GetAudioKey(AudioConf conf, string cfgid, string type)
    {
        JSONNode table = null;
        if (conf == AudioConf.Soldier)
        {
            table = GameData.instance.TableJsonDict["SoldierAudioConf"];
        }
        else if (conf == AudioConf.Skill)
        {
            table = GameData.instance.TableJsonDict["SkillAudioConf"];
        }
        else
        {
            table = GameData.instance.TableJsonDict["AudioConf"];
        }

        string audioKey = table[cfgid][type];

        return audioKey;
    }

    float time;
    public void PlayOneShot(AudioConf conf, string cfgid, string type, float pitch = 1.0f)
    {
        if (!IsSoundOn)
            return;

        string key = GetAudioKey(conf, cfgid, type);

        if (!CheckPlay(key))
            return;

        time = Time.time;
        var audioConf = GameData.instance.TableJsonDict["AudioConf"];
        var audioData = audioConf[key];
        JSONNode audioPaths = audioData["audioName"];
        int random = Random.Range(0, audioPaths[0].Count);
        string audioName = audioPaths[0][random];

        if (audioName == null)
            return;

        if (audioClipOneShotDict.TryGetValue(key, out Dictionary<string, AudioClip> audioClips))
        {
            if (audioClips.TryGetValue(audioName, out AudioClip audioClip))
            {
                playOneShotHandle(audioClip, audioData["volum"].AsFloat / 1000, key, audioData["playInterval"].AsFloat / 1000, audioData["audioType"], pitch);
                return;
            }
        }

        ResMgr.instance.LoadAudioClipAsync(audioName, (audio) =>
        {
            if (!audioClipOneShotDict.ContainsKey(key))
            {
                Dictionary<string, AudioClip> keyValuePairs = new Dictionary<string, AudioClip>();
                audioClipOneShotDict.Add(key, keyValuePairs);
            }

            if (!audioClipOneShotDict[key].ContainsKey(audioName))
            {
                audioClipOneShotDict[key].Add(audioName, audio);
            }

            if (!CheckPlay(key))
                return;

            playOneShotHandle(audio, audioData["volum"].AsFloat / 1000, key, audioData["playInterval"].AsFloat / 1000, audioData["audioType"], pitch);
        });
    }

    public void PlayOneShot(string audioName, float pitch = 1.0f)
    {
        if (!IsSoundOn)
            return;

        if (string.IsNullOrEmpty(audioName))
            return;

        var audioConf = GameData.instance.TableJsonDict["AudioConf"];
        var audioData = audioConf[audioName];

        if (audioData == null || audioData["audioName"][0] == null || audioData["audioName"][0][0] == null)
            return;

        if (audioClipNoGroupOneShotDict.TryGetValue(audioData["audioName"][0][0], out AudioClip audioClip))
        {
            playOneShotHandle(audioClip, audioData["volum"].AsFloat / 1000, audioName, audioData["playInterval"].AsFloat / 1000, audioData["audioType"], pitch);
            return;
        }

        ResMgr.instance.LoadAudioClipAsync(audioData["audioName"][0][0], (audio) =>
        {
            if (!audioClipNoGroupOneShotDict.ContainsKey(audioName))
            {
                audioClipNoGroupOneShotDict.Add(audioName, audio);
            }

            playOneShotHandle(audio, audioData["volum"].AsFloat / 1000, audioName, audioData["playInterval"].AsFloat / 1000, audioData["audioType"], pitch);
        });
    }

    //���key���Ƿ���Բ��ţ��Ƿ���CD
    private bool CheckPlay(string key)
    {
        if(key == null)
            return false;

        for (int i = 0; i < m_channels.Length; i++)
        {
            //������ڲ���ͬһ��Ƭ�Σ����Ҹողſ�ʼ����ֱ���˳�����
            var channels = m_channels[i];
            //Debug.Log($"keyOnTime��{channels.keyOnTime}, Time.time - channels.intervalTime:{Time.time - channels.intervalTime},  {channels.keyOnTime >= Time.time - channels.intervalTime}");
            //if (channels.channel.isPlaying &&
            //     //channels.channel.clip == clip &&
            //     channels.key == key &&
            //     channels.keyOnTime >= Time.time - channels.intervalTime)
            //    return false;

            //ͬһ�������Ƿ����ڲ��Ŷ���intervalTime�ļ��ʱ��
            if (channels.key == key && channels.keyOnTime >= Time.time - channels.intervalTime)
            {
                return false;
            }
        }

        return true;
    }

    //����һ�Σ�����Ϊ��ƵƬ�Ρ��������������ٶ�
    //���������Ҫ������Ч����˿�������Ч������߼�
    private int playOneShotHandle(AudioClip clip, float volume, string key, float intervalTime, int audioType, float pitch = 1.0f)
    {
       
        //��������Ƶ���������Ƶ������ֱ�Ӳ�������Ƶ�����˳�
        //���û�п���Ƶ�������ҵ��ʼ���ŵ�Ƶ����oldest�����Ժ�ʹ��
        //int oldest = -1;
        //float time = 10000000.0f;
        for (int i = 0; i < m_channels.Length; i++)
        {
            //if (m_channels[i].channel.loop == false &&
            //   m_channels[i].channel.isPlaying &&
            //   m_channels[i].keyOnTime < time)
            //{
            //    oldest = i;
            //    time = m_channels[i].keyOnTime;
            //}

            if (!m_channels[i].channel.isPlaying)
            {
                m_channels[i].channel.clip = clip;
                m_channels[i].channel.volume = volume;
                m_channels[i].channel.pitch = pitch;
                m_channels[i].key = key;
                m_channels[i].intervalTime = intervalTime;
                m_channels[i].audioType = audioType;
                //m_channels[i].channel.panStereo = pan;
                m_channels[i].channel.loop = false;
                m_channels[i].channel.Play();
                m_channels[i].keyOnTime = Time.time;
                return i;
            }
        }
        ////���е�����˵��û�п��Ƶ�������µ���Ƶ�������粥������Ƶ
        //if (oldest >= 0)
        //{
        //    m_channels[oldest].channel.clip = clip;
        //    m_channels[oldest].channel.volume = volume;
        //    m_channels[oldest].channel.pitch = pitch;
        //    m_channels[oldest].key = key;
        //    m_channels[oldest].intervalTime = intervalTime;
        //    //m_channels[oldest].channel.panStereo = pan;
        //    m_channels[oldest].channel.loop = false;
        //    m_channels[oldest].channel.Play();
        //    m_channels[oldest].keyOnTime = Time.time;
        //    return oldest;
        //}
        return -1;
    }

    public void PlayLoop(string audioName, float pitch = 1.0f)
    {
        var audioConf = GameData.instance.TableJsonDict["AudioConf"];
        var audioData = audioConf[audioName];

        if (audioData == null)
        {
            return;
        }

        if (audioData == null || audioData["audioName"][0] == null || audioData["audioName"][0][0] == null)
            return;

        if (audioClipLoopDict.TryGetValue(audioData["audioName"][0][0], out AudioClip audioClip))
        {
            playLoopHandle(audioClip, audioData["volum"].AsFloat / 1000, audioName, audioData["audioType"], pitch);
            return;
        }

        ResMgr.instance.LoadAudioClipAsync(audioData["audioName"][0][0], (audio) =>
        {
            if (!audioClipLoopDict.ContainsKey(audioName))
            {
                audioClipLoopDict.Add(audioName, audio);
            }

            playLoopHandle(audio, audioData["volum"].AsFloat / 1000, audioName, audioData["audioType"], pitch);
        });
    }

    //ѭ�����ţ����ڲ��ų�ʱ��ı������֣�������ʽ��Լ�һЩ
    private int playLoopHandle(AudioClip clip, float volume, string key, int audioType, float pitch = 1.0f)
    {
        for (int i = 0; i < m_channels.Length; i++)
        {
            if (!m_channels[i].channel.isPlaying)
            {
                m_channels[i].channel.clip = clip;
                m_channels[i].channel.volume = IsBgmOn ? volume : 0;
                m_channels[i].channel.pitch = pitch;
                m_channels[i].key = key;
                m_channels[i].audioType = audioType;
                //m_channels[i].channel.panStereo = pan;
                m_channels[i].channel.loop = true;
                m_channels[i].channel.Play();
                m_channels[i].keyOnTime = Time.time;
                return i;
            }
        }
        return -1;
    }

    public void SwitchBgmVolume(bool value)
    {
        IsBgmOn = value;
        PlayerPrefs.SetInt("isOnBGM", value ? 1 : 0);

        foreach (var channel in m_channels)
        {
            if (channel.audioType == 1)
            {
                var audioConf = GameData.instance.TableJsonDict["AudioConf"];
                var audioData = audioConf[channel.key];
                channel.channel.volume = value ? audioData["volum"].AsFloat / 1000 : 0;
            }
        }
    }

    public void SwitchSoundVolume(bool value)
    {
        IsSoundOn = value;
        PlayerPrefs.SetInt("isOnSound", value ? 1 : 0);

        foreach (var channel in m_channels)
        {
            if (channel.audioType == 2)
            {
                StopByKey(channel.key);
            }
        }
    }


    public void StopByKey(string key)
    {
        foreach (CHANNEL channel in m_channels)
        {
            if (channel.key == key)
            {
                channel.channel.Stop();
            }
        }
    }

    //ֹͣ������Ƶ
    public void StopAll()
    {
        foreach (CHANNEL channel in m_channels)
            channel.channel.Stop();
    }
    //����Ƶ��IDֹͣ��Ƶ
    public void Stop(int id)
    {
        if (id >= 0 && id < m_channels.Length)
        {
            m_channels[id].channel.Stop();
        }
    }
}
#endif