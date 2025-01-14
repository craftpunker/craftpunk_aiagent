using UnityEngine;
using UnityEngine.UI;
public class BloodImage : RawImage {
    private Slider _BloodSlider;
    protected override void OnRectTransformDimensionsChange()
    {  
        base.OnRectTransformDimensionsChange();
        //��ȡѪ��  
        if (_BloodSlider == null)
            _BloodSlider = transform.parent.parent.GetComponent<Slider>();
        //��ȡѪ����ֵ  
        if (_BloodSlider != null)
        {   
            //ˢ��Ѫ������ʾ  
            float value = _BloodSlider.value;
            uvRect = new Rect(0, 0, value, 1);
        }
    }
}