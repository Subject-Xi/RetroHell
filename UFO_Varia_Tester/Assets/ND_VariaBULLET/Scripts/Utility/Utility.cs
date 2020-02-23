#region Script Synopsis
    //Utitilies class contains common static helper methods used throughout the codebase.
    //CalcObject class containts common static helper methods with common calculations used by shots and firing scripts.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public static class Utilities
    {
        public static void Warn(string message, params object[] senderObjects)
        {
            string objects = "";

            if (senderObjects != null)
                foreach (object item in senderObjects)
                    objects += item.ToString() + "; ";

            Debug.Log("WARNING: " + message + " | Object(s): " + objects);
        }

        public static bool IsEditorMode()
        {
            if (!Application.isPlaying)
                return true;
            else
                return false;
        }

        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }
    }

    public static class CalcObject
    {
        public static bool IsOutBounds(Transform transform)
        {
            Vector3 tmpPos = Camera.main.WorldToScreenPoint(transform.position);
            float padding = GlobalShotManager.Instance.OutBoundsRange;

            if (tmpPos.x < 0 - padding || tmpPos.x > Screen.width + padding || tmpPos.y < 0 - padding || tmpPos.y > Screen.height + padding)
                return true;
            else
                return false;
        }

        public static Vector2 RotationToShotVector(float rotationAngle)
        {
            return new Vector2(Mathf.Cos(rotationAngle * Mathf.Deg2Rad), Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
        }

        public static Quaternion VectorToRotationSlerp(Quaternion srcRotation, Vector3 targetPos, float slerpSpeed)
        {      
            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            Quaternion rotationTarget = Quaternion.AngleAxis(angle, Vector3.forward);
            return Quaternion.Slerp(srcRotation, rotationTarget, Time.deltaTime * slerpSpeed);
        }

        public static float AngleBetweenVectors(Vector2 from, Vector2 to)
        {
            return Mathf.Atan2(to.y - from.y, to.x - from.x) * 180 / Mathf.PI;
        }
    }
}