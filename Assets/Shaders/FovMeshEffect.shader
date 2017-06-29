Shader "CameraEffect/FovMeshEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightPos("LightPos",Vector) = (0,0,0,0)
		_Radius("Radius",Vector) = (0,0,0,0)
		_Alpha("Alpha",Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _LightPos;
			float2 _Radius;
			float _Alpha;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float2 dir = i.uv - _LightPos;
				float dis = min(1, length(float2(dir.x / _Radius.x, dir.y / _Radius.y)));
				col *= 1 - pow(dis,2);
				col *= _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
