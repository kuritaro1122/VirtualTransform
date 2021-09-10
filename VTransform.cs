using UnityEngine;
using System;

namespace VirtualTransform {
    [System.Serializable]
    public class VTransform {
        private enum OptionType { transform, vector3, vector3Func }
        [SerializeField] OptionType optionType = OptionType.vector3Func;
        [SerializeField] Transform axisTransform = null;
        [SerializeField] Vector3 axisPosition = default;
        [SerializeField] Vector3 axisRotation = default;
        private Func<Vector3> axisPositionFunc = () => Vector3.zero;
        private Func<Quaternion> axisRotationFunc = () => Quaternion.identity;

        public VTransform(Transform axisTransform) {
            if (axisTransform != null) {
                this.axisTransform = axisTransform;
                optionType = OptionType.transform;
            } else WarningLog();
        }
        public VTransform(Vector3 axisPosition, Quaternion axisRotation) {
            this.axisPosition = axisPosition;
            this.axisRotation = axisRotation.eulerAngles;
            this.optionType = OptionType.vector3;
        }
        public VTransform(Vector3 axisPosition, Vector3 axisRotation) : this(axisPosition, Quaternion.Euler(axisRotation)) {}
        public VTransform(Func<Vector3> axisPosition, Func<Quaternion> axisRotation) {
            this.axisPositionFunc = axisPosition;
            this.axisRotationFunc = axisRotation;
            this.optionType = OptionType.vector3Func;
        }
        public VTransform() : this(Vector3.zero, Quaternion.identity) {}

        public void Set(Transform axisTransform) {
            if (axisTransform != null) {
                this.axisTransform = axisTransform;
                optionType = OptionType.transform;
            } else WarningLog();
        }
        public void Set(Vector3 axisPosition, Quaternion axisRotation) {
            this.axisPosition = axisPosition;
            this.axisRotation = axisRotation.eulerAngles;
            this.optionType = OptionType.vector3;
        }
        public void Set(Vector3 axisPosition, Vector3 axisRotation) => Set(axisPosition, Quaternion.Euler(axisRotation));
        public void Set(Func<Vector3> axisPosition, Func<Quaternion> axisRotation) {
            this.axisPositionFunc = axisPosition;
            this.axisRotationFunc = axisRotation;
            this.optionType = OptionType.vector3Func;
        }

        public Vector3 Position {
            get {
                switch (optionType) {
                    case OptionType.transform:
                        if (axisTransform == null) break;
                        return axisTransform.position;
                    case OptionType.vector3:
                        return axisPosition;
                    case OptionType.vector3Func:
                        return axisPositionFunc();
                }
                WarningLog();
                return default;
            }
        }
        public Quaternion Rotation {
            get {
                switch (optionType) {
                    case OptionType.transform:
                        if (axisTransform == null) break;
                        return axisTransform.rotation;
                    default:
                    case OptionType.vector3:
                        return Quaternion.Euler(axisRotation);
                    case OptionType.vector3Func:
                        return axisRotationFunc();
                }
                WarningLog();
                return default;
            }
        }
        public Vector3 WorldToLocalMove(Vector3 position) {
            return WorldToLocalMatrix4x4() * position;
        }
        public Vector3 WorldToLocalPosition(Vector3 position) {
            return WorldToLocalMatrix4x4() * (position - this.Position);
        }
        public Vector3 LocalToWorldMove(Vector3 localPosition) {
            return LocalToWorldMatrix4x4() * localPosition;
        }
        public Vector3 LocalToWorldPosition(Vector3 localPosition) {
            return (Vector3)(LocalToWorldMatrix4x4() * localPosition) + this.Position;
        }
        public Matrix4x4 WorldToLocalMatrix4x4() {
            return Matrix4x4.Inverse(LocalToWorldMatrix4x4());
        }
        public Matrix4x4 LocalToWorldMatrix4x4() {
            Matrix4x4 matrix = Matrix4x4.identity;
            switch (optionType) {
                case OptionType.transform:
                    if (axisTransform != null)
                        matrix.SetTRS(Vector3.zero, axisTransform.rotation, Vector3.one);
                    break;
                case OptionType.vector3:
                    matrix.SetTRS(Vector3.zero, Quaternion.Euler(axisRotation), Vector3.one);
                    break;
                case OptionType.vector3Func:
                    matrix.SetTRS(Vector3.zero, axisRotationFunc(), Vector3.one);
                    break;
            }
            return matrix;
        }
        private void WarningLog() {
            Debug.LogWarningFormat("Warning:RelativePositionGetter/AxisOption/axisTransform is null.");
            //optionType = OptionType.vector3Func;
        }
        public Vector3 LocalToWorldDirection(Vector3 localDirection) =>
            LocalToWorldPosition(localDirection) - LocalToWorldPosition(Vector3.zero);
        public Vector3 WorldToLocalDirection(Vector3 direction) =>
            WorldToLocalPosition(direction) - WorldToLocalPosition(Vector3.zero);
    }
}