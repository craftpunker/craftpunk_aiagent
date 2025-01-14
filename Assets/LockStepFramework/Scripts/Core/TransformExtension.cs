#if _CLIENTLOGIC_
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TransformExtension {
    public static void ReSet (this Transform tr) {
        tr.localPosition = Vector3.zero;
    }
    public static void ReSet (this GameObject tr) {
        tr.transform.localPosition = Vector3.zero;
    }

    public static void ResetLocal (this Transform tr) {
        tr.localPosition = Vector3.zero;
        tr.transform.localRotation = Quaternion.identity;
        tr.transform.localScale = Vector3.one;
    }
    public static void SetScale (this Transform tr, Vector3 scale) {
        tr.localScale = scale;
    }

    public static void SetLocalScaleX (this Transform tr, float x) {
        var scale = tr.localScale;
        scale.x = x;
        tr.localScale = scale;
    }
    public static void SetLocalScaleY (this Transform tr, float y) {
        var scale = tr.localScale;
        scale.y = y;
        tr.localScale = scale;
    }
    public static void SetLocalScale (this Transform tr, float size) {
        var scale = tr.localScale;
        scale.x = size;
        scale.y = size;
        tr.localScale = scale;
    }

    public static void SetWorldPosX (this Transform tr, float x) {
        var worldPos = tr.position;
        worldPos.x = x;
        tr.position = worldPos;
    }

    public static void SetWorldPosY (this Transform tr, float y) {
        var worldPos = tr.position;
        worldPos.y = y;
        tr.position = worldPos;
    }
    public static void SetWorldPosZ (this Transform tr, float z) {
        var worldPos = tr.position;
        worldPos.z = z;
        tr.position = worldPos;
    }

    public static void SetWorldPos (this Transform tr, float x, float y, float z) {
        var worldPos = tr.position;
        worldPos.x = x;
        worldPos.y = y;
        worldPos.z = z;
        tr.position = worldPos;
    }

    public static void SetLocalPosX (this Transform tr, float x) {
        var localPos = tr.localPosition;
        localPos.x = x;
        tr.localPosition = localPos;
    }

    public static void SetLocalPosY (this Transform tr, float y) {
        var localPos = tr.localPosition;
        localPos.y = y;
        tr.localPosition = localPos;
    }

    public static void SetLocalPosZ (this Transform tr, float z) {
        var localPos = tr.localPosition;
        localPos.z = z;
        tr.localPosition = localPos;
    }

    public static void SetLocalPos (this Transform tr, float x, float y, float z) {
        var localPos = tr.localPosition;
        localPos.x = x;
        localPos.y = y;
        localPos.z = z;
        tr.localPosition = localPos;
    }

    public static void SetLocalPos (this GameObject obj, float x, float y, float z) {
        if (obj == null) {
            return;
        }
        var localPos = obj.transform.localPosition;
        localPos.x = x;
        localPos.y = y;
        localPos.z = z;
        obj.transform.localPosition = localPos;
    }

    public static Vector2 GetTransformRectPos (this Transform rt, float x) {
        if (rt) {
            RectTransform rect = rt.GetComponent<RectTransform> ();
            if (rect != null) {
                return rect.anchoredPosition;
            }
        }
        return Vector2.zero;
    }

    public static void SetRectPosX (this RectTransform rt, float x) {
        var pos = rt.anchoredPosition;
        pos.x = x;
        rt.anchoredPosition = pos;
    }
    public static void SetRectPosXY (this RectTransform rt, float x, float y) {
        rt.anchoredPosition = new Vector2 (x, y);
    }
    public static void SetRectPosScale (this RectTransform rt, RectTransform targetRt, float rate) {
        rt.position = targetRt.position;
        var x = targetRt.sizeDelta.x * rate;
        var y = targetRt.sizeDelta.y * rate;
        x = x == 0 ? 1 : x;
        y = y == 0 ? 1 : y;
        var size = Mathf.Min (x, y);
        rt.localScale = new Vector3 (size, size, 1);
    }
    public static void SetRectPosY (this RectTransform rt, float y) {
        var pos = rt.anchoredPosition;
        pos.y = y;
        rt.anchoredPosition = pos;
    }

    public static void SetRectSizeX (this RectTransform rt, float x) {
        var size = rt.sizeDelta;
        size.x = x;
        rt.sizeDelta = size;
    }

    public static void SetRectSizeY (this RectTransform rt, float y) {
        var size = rt.sizeDelta;
        size.y = y;
        rt.sizeDelta = size;
    }
    public static void SetTransformRectPos (this Transform rt, float x, float y) {
        if (rt) {
            RectTransform rect = rt.GetComponent<RectTransform> ();
            if (rect != null) {
                var size = rect.sizeDelta;
                size.x = x;
                size.y = y;
                rect.sizeDelta = size;
            }
        }

    }

    public static void SetActiveEx (this GameObject obj, bool active) {
        if (obj.activeSelf != active) {
            obj.SetActive (active);
        }
    }
    public static void SetActiveEx (this Transform obj, bool active) {
        if (obj.gameObject.activeSelf != active) {
            obj.gameObject.SetActive (active);
        }
    }
    public static void SetActiveEx (this Component comp, bool active) {
        if (comp.gameObject.activeSelf != active) {
            comp.gameObject.SetActive (active);
        }
    }

    public static void SetSpriteRendererActiveEx (this SpriteRenderer spriteRenderer, bool active) {
        spriteRenderer.enabled = active;
    }

    public static void FastSetLocalScale (this Transform tr, float x, float y, float z) {
        tr.localScale = new Vector3 (x, y, z);
    }
    public static void FastSetLocalScale (this GameObject obj, float x, float y, float z) {
        obj.transform.localScale = new Vector3 (x, y, z);
    }
    public static void FastSetPosWithScale (this Transform obj, float x, float y, float z, float scale) {
        obj.position = new Vector3 (x / scale, y / scale, z / scale);
    }
    public static void FastSetPos (this Transform obj, float x, float y, float z) {
        obj.position = new Vector3 (x, y, z);
    }
    public static void FastSetPos (this GameObject obj, float x, float y, float z) {
        obj.transform.position = new Vector3 (x, y, z);
    }
    public static void FastSetLocalPos (this Transform obj, float x, float y, float z) {
        obj.localPosition = new Vector3 (x, y, z);
    }
    public static void FastSetLocalPos (this GameObject obj, float x, float y, float z) {
        obj.transform.localPosition = new Vector3 (x, y, z);
    }
    public static void FastEulerRotation (this GameObject obj, float x, float y, float z) {
        obj.transform.rotation = Quaternion.Euler (x, y, z);
    }
    public static void FastEulerRotation (this Transform obj, float x, float y, float z) {
        obj.rotation = Quaternion.Euler (x, y, z);
    }
    public static void FastEulerLocalRotation (this GameObject obj, float x, float y, float z) {
        obj.transform.localRotation = Quaternion.Euler (x, y, z);
    }
    public static void FastEulerLocalRotation (this Transform obj, float x, float y, float z) {
        obj.localRotation = Quaternion.Euler (x, y, z);
    }
    public static void FastSetIdentityRotation (this Transform obj) {
        obj.localEulerAngles = Vector3.zero;
    }

    public static void FastLookRotationUp (this Transform obj, float x, float y, float z) {
        var q = new Vector3 (x, y, z).normalized;
        if (q != Vector3.zero) {
            obj.localRotation = Quaternion.LookRotation (q, Vector3.up);
        }
    }
    public static void FastLookRotationUp (this GameObject obj, float x, float y, float z) {
        var q = new Vector3 (x, y, z).normalized;
        if (q != Vector3.zero) {
            obj.transform.localRotation = Quaternion.LookRotation (q, Vector3.up);
        }
    }

    public static float FastCalculateDistance (this GameObject obj, GameObject targetObj) {
        if (targetObj == null) {
            return 1000;
        }
        return Vector3.Distance (obj.transform.position, targetObj.transform.position);
    }

    public static void FastLookAtTargetPosition (this GameObject obj, Vector3 targetPos) {
        obj.transform.LookAt (targetPos);
    }
    public static void FastLookAtMainCamera (this GameObject obj) {
        obj.transform.LookAt (Camera.main.transform);
    }

    public static void FastLookAtTarget (this GameObject obj, Transform targetPos) {
        obj.transform.LookAt (targetPos);
    }

    public static void FastLookForwardRotate (this Transform tran, float x, float y, float z, float angleStep) {
        var q = new Vector3 (x, y, z);
        if (q != Vector3.zero) {
            var endRotate = Quaternion.LookRotation (q, Vector3.up);
            tran.localRotation = Quaternion.Slerp (tran.localRotation, endRotate, angleStep);
        }
    }

    public static bool RotateToward (this Transform tran, float x, float y, float z, float angleStep) {
        var q = new Vector3 (x, y, z);
        if (q != Vector3.zero) {
            var endRotate = Quaternion.LookRotation (q, Vector3.up);
            tran.localRotation = Quaternion.Slerp (tran.localRotation, endRotate, angleStep);
        }
        return tran.forward == q;
    }

    public static Vector3 FastGetCircleIntersectionPoint (this GameObject obj, Vector3 targetPos, float r) {
        var result = Vector3.zero;
        var selfPos = obj.transform.position;
        var p = (targetPos - selfPos).normalized * r;
        return selfPos + p;
    }

    public static void FastSetParent (this Transform tran, Transform parent, bool worldPositionStays) {
        tran.SetParent (parent, worldPositionStays);
    }
    public static void FastSetParent (this GameObject go, Transform parent, bool worldPositionStays) {
        go.transform.SetParent (parent, worldPositionStays);
    }

    public static void SetImageEnable (this Image img, bool active) {
        if (img.enabled != active) {
            img.enabled = active;
        }
    }
    public static void SetTextEnable (this Text text, bool active) {
        if (text.enabled != active) {
            text.enabled = active;
        }
    }

    public static void SetParentEx (this Transform tran, Transform parent, bool worldPositionStays) {
        if (tran.parent != null && tran.parent.Equals (parent)) {
            //
        } else {
            tran.SetParent (parent, worldPositionStays);
        }
    }

    public static void SetSpriteRenderColorEx (this SpriteRenderer tran, float r, float g, float b, float a) {
        if (tran != null) {
            tran.color = new Color (r, g, b, a);
        }
    }
    public static void FastSetName (this GameObject go, string newName) {
        go.name = newName;
    }
    public static Transform FastFindTransform (this Transform go, string path) {
        return go.Find (path);
    }
    public static Transform FastFindTransform (this GameObject go, string path) {
        return go.transform.Find (path);
    }
    public static Component FastAddComponentType (this Transform go, Type componentType) {
        return go.gameObject.AddComponent (componentType);
    }
    public static Component FastAddComponentType (this GameObject go, Type componentType) {
        return go.AddComponent (componentType);
    }
    public static Component FastAddComponent (this Transform go, string componentType) {
        return go.gameObject.AddComponent (Type.GetType (componentType));
    }
    public static Component FastAddComponent (this GameObject go, string componentType) {
        return go.AddComponent (Type.GetType (componentType));
    }
    public static Component FastGetComponentType (this Component go, string path, Type componentType) {
        if (go == null) {
            return null;
        }
        var tran = go;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.transform.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static Component FastGetComponentType (this Transform go, string path, Type componentType) {
        if (go == null) {
            return null;
        }
        var tran = go;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static Component FastGetComponentType (this GameObject go, string path, Type componentType) {
        if (go == null) {
            return null;
        }
        var tran = go.transform;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.transform.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static Component FastGetComponent (this Component go, string path, string componentType) {
        if (go == null || string.IsNullOrEmpty (componentType)) {
            return null;
        }
        var tran = go;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.transform.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static Component FastGetComponent (this Transform go, string path, string componentType) {
        if (go == null || string.IsNullOrEmpty (componentType)) {
            return null;
        }
        var tran = go;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static Transform[] FastGetAllChildTf (this Transform go) {
        return go.GetComponents<Transform> ();
    }
    public static Component FastGetComponent (this GameObject go, string path, string componentType) {
        if (go == null || string.IsNullOrEmpty (componentType)) {
            return null;
        }
        var tran = go.transform;
        if (string.IsNullOrEmpty (path) == false) {
            tran = go.transform.Find (path);
        }
        if (tran == null) {
            return null;
        }
        return tran.GetComponent (componentType);
    }
    public static MeshRenderer FastGetMeshRenderInChildren (this Transform tran, bool includeInactive) {
        return tran.GetComponentInChildren<MeshRenderer> (includeInactive);
    }
    public static MeshRenderer FastGetMeshRenderInChildren (this GameObject go, bool includeInactive) {
        return go.GetComponentInChildren<MeshRenderer> (includeInactive);
    }
    public static void FastHideTransformInChildren (this GameObject go, bool state, string filterName) {
        FastHideTransformInChildren (go.transform, state, filterName);
    }
    public static void FastHideTransformInChildren (this Transform tran, bool state, string filterName) {
        for (int i = 0; i < tran.childCount; i++) {
            var trans = tran.GetChild (i);
            if (filterName != null && filterName != "") {
                if (trans.name == filterName) {
                    continue;
                }
            }
            trans.SetActiveEx (state);
        }
    }
    public static string FastGetParentName (this GameObject go) {
        return go.transform.parent.name;
    }
    public static Vector2 FastGetClickBoxXY (this GameObject go) {
        var pName = go.FastGetParentName ();
        var points = pName.Split ('_');
        return new Vector2 (int.Parse (points[0]), int.Parse (points[1]));
    }

    public static GameObject FastInstanceGameObject (GameObject go) {
        return GameObject.Instantiate<GameObject> (go);
    }

    public static bool IsInEditorMode () {
        return Application.platform == RuntimePlatform.WindowsEditor;
    }

    public static bool IsInUIArea (this Transform go, float x, float y, float r) {
        bool isIn = false;
        var distance = Vector3.Distance (new Vector3 (go.position.x, go.position.y, 0), new Vector3 (x, y));
        if (distance < r) {
            isIn = true;
        }
        return isIn;
    }

}
#endif