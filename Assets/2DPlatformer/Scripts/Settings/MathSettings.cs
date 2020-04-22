using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance {

	/// <summary> 
	/// Used to optimize the calculation of mathematical operations.
	/// Has calculated cos and sin angle values.
	/// </summary>
	[CreateAssetMenu(fileName = "MathSettings", menuName = "Game Balance/Settings/MathSettings")]
	public class MathSettings : ScriptableObject {
		[Header ("Sin/Cos Settings")]
		[SerializeField] float StartAngle = -60f;
		[SerializeField] float EndAngle = 60f;
		[SerializeField] float RoundTo = 0.1f;

		[SerializeField, HideInInspector] float[] Cos;
		[SerializeField, HideInInspector] float[] Sin;
		[SerializeField, HideInInspector] int IndexOffset;


		[ContextMenu("GenerateValues")]
		void GenerateValues () {
			IndexOffset = Mathf.Abs(Mathf.RoundToInt(StartAngle / RoundTo));
			Cos = new float[IndexOffset * 2 + 1];
			Sin = new float[IndexOffset * 2 + 1];
			float angle = StartAngle;
			float cosAngle = 0;
			float sinAngle = 0;

			int startIndex = Mathf.RoundToInt(angle / RoundTo);;

			while (angle < EndAngle) {
				GetAndAddInArray(angle, out cosAngle, out sinAngle);
				angle = Mathf.MoveTowards(angle, EndAngle, RoundTo);
			} 
		}

		void GetAndAddInArray (float angle, out float cos, out float sin) {
			var angleRad = angle * Mathf.Deg2Rad;
			var cosAngle = Mathf.Cos(angleRad);
			var sinAngle = Mathf.Sin(angleRad);
			var index = Mathf.RoundToInt(angle / RoundTo);

			Cos[IndexOffset + index] = cosAngle;
			Sin[IndexOffset + index] = sinAngle;

			cos = cosAngle;
			sin = sinAngle;
		}

		public float GetCos (float angle) {
			int index = Mathf.RoundToInt(angle / RoundTo);
			return Cos[IndexOffset + index];
		}

		public float GetSin (float angle) {
			int index = (int)(angle / RoundTo);
			return Sin[IndexOffset + index];
		}
	}
}
