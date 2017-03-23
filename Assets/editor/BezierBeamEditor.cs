using UnityEngine;
using UnityEditor;

public class BezierBeamEditor : ShaderGUI {

	const int POINT_COUNT = 4;
	const string POINTS_ARRAY_NAME = "curve_points";

	Vector4[] points;
	
	public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties) {
		foreach (MaterialProperty property in properties) {
			editor.ShaderProperty(property, property.displayName);
		}
		Material mat = editor.target as Material;
		Vector4[] points = mat.GetVectorArray(POINTS_ARRAY_NAME);
		if (points == null) {
			points = this.points;
			if (points == null) {
				points = new Vector4[POINT_COUNT];
			}
		}
		EditorGUILayout.LabelField("Curve Points");
		EditorGUI.indentLevel ++;
		for (int p = 0; p < points.Length; p ++) {
			Vector4 point = EditorGUILayout.Vector3Field(GUIContent.none, points[p]);
			point.w = 1;
			points[p] = point;
		}
		EditorGUI.indentLevel --;
		mat.SetVectorArray(POINTS_ARRAY_NAME, points);
		this.points = points;
	}

}