using UnityEngine;
using System;

namespace VirtualTransform {
	//===== VTransformで選択 =====
	[System.Serializable]
	public class VPosition {
		[SerializeField] VTransform axis;
		[SerializeField] Vector3 localPosition;
		public Vector3 GetPosition() {
			return axis.LocalToWorldPosition(localPosition);
		}
		public Vector3 GetMoveFromAxis() {
			return axis.LocalToWorldMove(localPosition);
		}
	}
	[System.Serializable]
	public class VDirection {
		[SerializeField] VPosition toPoint;
		[SerializeField] VPosition fromPoint;
		public Vector3 GetDirection() {
			return toPoint.GetPosition() - fromPoint.GetPosition();
		}
	}
	[System.Serializable]
	public class VRotation {
		[SerializeField] VDirection direction;
		[SerializeField] VDirection upwards;
		public Quaternion GetRotation() {
			return Quaternion.LookRotation(direction.GetDirection(), upwards.GetDirection());
		}
	}

	//===== Enumで選択 =====
	[System.Serializable]
	public class VPosition_Enum<VTransformEnum> : IVPosition_Enum_isAdd where VTransformEnum : Enum {
		[SerializeField] VTransformEnum axis;
		[SerializeField] Vector3 localPosition;
		public Vector3 GetPosition(VTransform[] vTransformList) {
			return vTransformList[(int)(object)axis].LocalToWorldPosition(localPosition);
		}
		public Vector3 GetMoveFromAxis(VTransform[] vTransformList) {
			return vTransformList[(int)(object)axis].LocalToWorldMove(localPosition);
		}
	}
	[System.Serializable]
	public class VPositionPro_Enum<VTransformEnum> : IVPosition_Enum_isAdd where VTransformEnum : Enum {
		[SerializeField] VTransformEnum axis;
		[SerializeField] Vector3 rotation;
		[SerializeField] CoordinateConverter.CoordinateSystem coordinateSystem;
		[SerializeField] Vector3 localPosition;
		public Vector3 GetPosition(VTransform[] vTransformList) {
			var Axis = vTransformList[(int)(object)axis];
			Matrix4x4 rotateMat = Library.RotationMatrix4x4(rotation);
			Vector3 localPos = CoordinateConverter.CoordinateConvert(localPosition, coordinateSystem);
			return Axis.LocalToWorldMatrix4x4() * rotateMat * localPos;
		}
		public Vector3 GetMoveFromAxis(VTransform[] vTransformList) {
			VTransform Axis = vTransformList[(int)(object)axis];
			return this.GetPosition(vTransformList) - Axis.Position;
		}
	}
	[System.Serializable]
	public class VDirection_Enum<P> : IVDirection_Enum where P : IVPosition_Enum {
		[SerializeField] P fromPoint;
		[SerializeField] P toPoint;
		public Vector3 GetDirection(VTransform[] vTransformList) {
			return toPoint.GetPosition(vTransformList) - fromPoint.GetPosition(vTransformList);
		}
	}
	[System.Serializable]
	public class VRotation_Enum<D> : IVRotation_Enum where D : IVDirection_Enum {
		[SerializeField] D direction;
		[SerializeField] D upwards;
		public Quaternion GetRotation(VTransform[] vTransformList) {
			return Quaternion.identity;
		}
	}
	public interface IVPosition_Enum {
		Vector3 GetPosition(VTransform[] vTransformList);
	}
	public interface IVPosition_Enum_isAdd : IVPosition_Enum {
		Vector3 GetMoveFromAxis(VTransform[] vTransformList);
	}
	public interface IVDirection_Enum {
		Vector3 GetDirection(VTransform[] vTransformList);
	}
	public interface IVRotation_Enum {
		Quaternion GetRotation(VTransform[] vTransformList);
	}

	//===== 独自のVector3クラスを使用 ===== //VariableValueなどを使う
	[System.Serializable]
	public class VPosition<Vector3Class, Input> : IVPosition_isAdd<Input> where Vector3Class : IVector3Class<Input> {
		[SerializeField] VTransform axis;
		[SerializeField] Vector3Class localPosition;
		public Vector3 GetPosition(Input value) {
			Vector3 localPos = localPosition.GetVector3(value);
			return axis.LocalToWorldPosition(localPos);
		}
		public Vector3 GetMoveFromAxis(Input value) {
			return axis.LocalToWorldMove(localPosition.GetVector3(value));
		}
	}
	[System.Serializable]
	public class VPositionPro<Vector3Class, Input> : IVPosition_isAdd<Input> where Vector3Class : IVector3Class<Input> {
		[SerializeField] VTransform axis;
		[SerializeField] Vector3 rotation;
		[SerializeField] CoordinateConverter.CoordinateSystem coordinateSystem;
		[SerializeField] Vector3Class localPosition;
		public Vector3 GetPosition(Input value) {
			Matrix4x4 rotateMat = Library.RotationMatrix4x4(rotation);
			Vector3 localPos = CoordinateConverter.CoordinateConvert(localPosition.GetVector3(value), coordinateSystem);
			return axis.LocalToWorldMatrix4x4() * rotateMat * localPos;
		}
		public Vector3 GetMoveFromAxis(Input value) {
			return this.GetPosition(value) - axis.Position;
		}
	}
	[System.Serializable]
	public class VDirection<P, Input> : IVDirection<Input> where P : IVPosition<Input> {
		[SerializeField] P toPoint;
		[SerializeField] P fromPoint;
		public Vector3 GetDirection(Input value) {
			return toPoint.GetPosition(value) - fromPoint.GetPosition(value);
		}
	}
	[System.Serializable]
	public class VRotation<D, Input> : IVRotation<Input> where D : IVDirection<Input> {
		[SerializeField] D direction;
		[SerializeField] D upwards;
		public Quaternion GetRotation(Input value) {
			return Quaternion.LookRotation(direction.GetDirection(value), upwards.GetDirection(value));
		}
	}
	public interface IVector3Class<Input> {
		Vector3 GetVector3(Input value);
	}
	public interface IVPosition<Input> {
		Vector3 GetPosition(Input value);
	}
	public interface IVPosition_isAdd<Input> : IVPosition<Input>{
		Vector3 GetMoveFromAxis(Input value);
	}
	public interface IVDirection<Input> {
		Vector3 GetDirection(Input value);
	}
	public interface IVRotation<Input> {
		Quaternion GetRotation(Input value);
	}

	//===== Enumで選択 かつ 独自のVector3クラスを使用 =====
	[System.Serializable]
	public class VPosition_Enum<VTransformEnum, Vector3Class, Input> : IVPosition_Enum_isAdd<Input> where VTransformEnum : Enum where Vector3Class : IVector3Class<Input> {
		[SerializeField] VTransformEnum axis;
		[SerializeField] Vector3Class localPosition;
		public Vector3 GetPosition(VTransform[] vTransformList, Input value) {
			Vector3 localPos = localPosition.GetVector3(value);
			return vTransformList[(int)(object)axis].LocalToWorldPosition(localPos);
		}
		public Vector3 GetMoveFromAxis(VTransform[] vTransformList, Input value) {
			return vTransformList[(int)(object)axis].LocalToWorldMove(localPosition.GetVector3(value));
		}
	}
	[System.Serializable]
	public class VPositionPro_Enum<VTransformEnum, Vector3Class, Input> : IVPosition_Enum_isAdd<Input> where VTransformEnum : Enum where Vector3Class : IVector3Class<Input> {
		[SerializeField] VTransformEnum axis;
		[SerializeField] Vector3Class rotation;
		[SerializeField] CoordinateConverter.CoordinateSystem coordinateSystem;
		[SerializeField] Vector3Class localPosition;
		public Vector3 GetPosition(VTransform[] vTransformList, Input value) {
			var Axis = vTransformList[(int)(object)axis];
			Matrix4x4 rotateMat = Library.RotationMatrix4x4(rotation.GetVector3(value));
			Vector3 localPos = CoordinateConverter.CoordinateConvert(localPosition.GetVector3(value), coordinateSystem);
			return Axis.LocalToWorldMatrix4x4() * rotateMat * localPos;
		}
		public Vector3 GetMoveFromAxis(VTransform[] vTransformList, Input value) {
			var Axis = vTransformList[(int)(object)axis];
			return this.GetPosition(vTransformList, value) - Axis.Position;
		}
	}

	[System.Serializable]
	public class VDirection_Enum<P, Input> : IVDirection_Enum<Input> where P : IVPosition_Enum<Input> {
		[SerializeField] P toPoint;
		[SerializeField] P fromPoint;
		public Vector3 GetDirection(VTransform[] vTransformList, Input value) {
			return toPoint.GetPosition(vTransformList, value) - fromPoint.GetPosition(vTransformList, value);
		}
	}
	[System.Serializable]
	public class VRotation_Enum<D, Input> : IVRotation_Enum<Input> where D : IVDirection_Enum<Input> {
		[SerializeField] D direction;
		[SerializeField] D upwards;
		public Quaternion GetRotation(VTransform[] vTransformList, Input value) {
			return Quaternion.LookRotation(direction.GetDirection(vTransformList, value), upwards.GetDirection(vTransformList, value));
		}
	}
	public interface IVPosition_Enum<Input> {
		Vector3 GetPosition(VTransform[] vTransformList, Input value);
	}
	public interface IVPosition_Enum_isAdd<Input> : IVPosition_Enum<Input> {
		Vector3 GetMoveFromAxis(VTransform[] vTransformList, Input value);
	}
	public interface IVDirection_Enum<Input> {
		Vector3 GetDirection(VTransform[] vTransformList, Input value);
	}
	public interface IVRotation_Enum<Input> {
		Quaternion GetRotation(VTransform[] vTransformList, Input value);
	}

	[System.Serializable]
	public class VPositionAdd {
		[SerializeField] VPosition beginPosition;
		[SerializeField] VPosition[] addPosition;
		public Vector3 GetPosition() {
			Vector3 point = beginPosition.GetPosition();
			foreach (var mpoint in addPosition) point += mpoint.GetMoveFromAxis();
			return point;
		}
	}
	[System.Serializable]
	public class VDirectionAdd {
		[SerializeField] VPositionAdd fromPoint;
		[SerializeField] VPositionAdd toPoint;
		public Vector3 GetDirection() {
			return toPoint.GetPosition() - fromPoint.GetPosition();
		}
	}
	[System.Serializable]
	public class VRotationAdd {
		[SerializeField] VDirectionAdd direction;
		[SerializeField] VDirectionAdd upwards;
		public Quaternion GetRotation() {
			return Quaternion.LookRotation(direction.GetDirection(), upwards.GetDirection());
		}
	}

	[System.Serializable]
	public class VPositionAdd_Enum<P> : IVPosition_Enum where P : IVPosition_Enum_isAdd {
		[SerializeField] P beginPosition;
		[SerializeField] P[] addPosition;
		public Vector3 GetPosition(VTransform[] vTransformList) {
			Vector3 point = beginPosition.GetPosition(vTransformList);
			foreach (var mpoint in addPosition) point += mpoint.GetMoveFromAxis(vTransformList);
			return point;
		}
	}
	[System.Serializable]
	public class VPositionAdd<P, Input> : IVPosition<Input> where P : IVPosition_isAdd<Input>{
		[SerializeField] P beginPosition;
		[SerializeField] P[] addPosition;
		public Vector3 GetPosition(Input input) {
			Vector3 point = beginPosition.GetPosition(input);
			foreach (var mpoint in addPosition) point += mpoint.GetMoveFromAxis(input);
			return point;
		}
	}
	[System.Serializable]
	public class VPositionAdd_Enum<P, Input> : IVPosition_Enum<Input> where P : IVPosition_Enum_isAdd<Input> {
		[SerializeField] P beginPosition;
		[SerializeField] P[] addPosition;
		public Vector3 GetPosition(VTransform[] vTransformList, Input input) {
			Vector3 point = beginPosition.GetPosition(vTransformList, input);
			foreach (var mpoint in addPosition) point += mpoint.GetMoveFromAxis(vTransformList, input);
			return point;
		}
	}

	internal class Library {
		public static Matrix4x4 RotationMatrix4x4(Vector3 rotation) {
			Matrix4x4 matrix4x4 = Matrix4x4.identity;
			matrix4x4.SetTRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);
			return matrix4x4;
		}
	}
}
