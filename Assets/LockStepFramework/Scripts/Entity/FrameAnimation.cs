#if _CLIENTLOGIC_
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    public Texture2D spriteAtlas; // �ͼ����ͼ
    public int[] frameCounts; // ÿ��������֡��
    public int rows = 1;
    public int columns = 1;
    public float framesPerSecond = 10.0f;
    public Vector2[] offsets; // ÿ��������ͼ���е�ƫ����
    public Vector2 atlasSize = new Vector2(1024, 1024); // ͼ���Ŀ��͸�

    private Renderer rend;
    private int currentAnimation = 0;
    private float frameTimer = 0;
    private int currentFrame = 0;

    void Start()
    {
        rend = GetComponent<Renderer>();
        SetAnimation(0);
    }

    void Update()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= 1.0f / framesPerSecond)
        {
            frameTimer -= 1.0f / framesPerSecond;
            currentFrame = (currentFrame + 1) % frameCounts[currentAnimation];
            rend.material.SetFloat("_CurrentFrame", currentFrame);
        }
    }

    public void SetAnimation(int animationIndex)
    {
        if (animationIndex >= 0 && animationIndex < frameCounts.Length)
        {
            currentAnimation = animationIndex;
            rend.material.SetTexture("_MainTex", spriteAtlas);
            rend.material.SetFloat("_FrameCount", frameCounts[animationIndex]);
            rend.material.SetFloat("_Rows", rows);
            rend.material.SetFloat("_Columns", columns);
            rend.material.SetFloat("_OffsetX", offsets[animationIndex].x);
            rend.material.SetFloat("_OffsetY", offsets[animationIndex].y);
            rend.material.SetFloat("_AtlasWidth", atlasSize.x);
            rend.material.SetFloat("_AtlasHeight", atlasSize.y);
            currentFrame = 0;
            rend.material.SetFloat("_CurrentFrame", currentFrame);
        }
    }
}
#endif