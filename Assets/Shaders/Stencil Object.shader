// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/Stencil Object"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_FovTexture("FovTexture",2D) = "white"{}
		//_ChangeDegree("ChangeDegree",Float) = 0
		//_PlayerPos("PlayerPos",Vector) = (0,0,0,0)
		_WorldSize("WorldSize",Float) = 1
		_FovInfoTex("FovInfoTex",2D) = "white"{}
		_LightCount("LightCount",Float) = 1
		_Runing("Runing",Float) = 0
		//_ViewRadius("ViewRadius",Float) = 3
		//_ViewOffset("ViewOffset",Float) = 3
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend One OneMinusSrcAlpha
		/*Stencil{
			Ref 1
			Comp Equal
		}*/
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 world : TEXCOORD1;
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.world = mul(unity_ObjectToWorld, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _FovTexture;
			sampler2D _NextFovTexture;
			//float4 _PlayerPos;
			/*float _ViewRadius;
			float _ViewOffset;*/
			sampler2D _FovInfoTex;
			float _WorldSize;
			float _LightCount;
			uniform vector _Pos[10];
			float _Runing;
			//float _ChangeDegree;
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				float aSum = 0;
				float pixelSize = 1 / _LightCount;
				for(float i = 0;i<_LightCount;i++){
					float aNow = 0;
					float4 Info = tex2D(_FovInfoTex, float2( 0.5,pixelSize/2+pixelSize*i));
					//float2 _Pos = (Info.rg - float2(0.5,0.5)) * _WorldSize;
					float _ViewRadius = Info.g * _WorldSize;
					float _ViewOffset = Info.b * _WorldSize;
					float _ChangeDegree = Info.r;
					float2 pixelDir = IN.world.xy - _Pos[i].xy;
					float pixelDis = length(pixelDir);
					float pixelAngle = degrees(acos(pixelDir.y / pixelDis));
					if (pixelDir.x < 0) {
						pixelAngle = - pixelAngle;
					}
					pixelAngle /= 360;

					if (pixelDis > _ViewRadius + _ViewOffset) {
						aNow = 0;
					}
					else {
						float2 meshDistance;
						float meshNextDistance;
						meshDistance = tex2D(_FovTexture, float2(pixelAngle, pixelSize / 2 + pixelSize*i),0.003,0).rg * _ViewRadius;
						float2 A;
						if (pixelDis < meshDistance.x)
						{
							A = 1;
						}else if (pixelDis > meshDistance.x + _ViewOffset)
						{
							A = 0;
						}else
						{
							float overStepDistance = pixelDis - meshDistance.x;
							A = (_ViewOffset - overStepDistance) / _ViewOffset;
						}
						float nA;
						if (pixelDis < meshDistance.y)
						{
							nA = 1;
						}
						else if (pixelDis > meshDistance.y + _ViewOffset)
						{
							nA = 0;
						}
						else
						{
							float overStepDistance = pixelDis - meshDistance.y;
							nA = (_ViewOffset - overStepDistance) / _ViewOffset;
						}
						aNow = A *(1 - _ChangeDegree) + nA * _ChangeDegree;
					}
					aSum += aNow;
				}
				c.a = min(1,aSum);
				if (_Runing == 1) {
					c.a = 1;
				}
				c.rgb *= c.a;

				return c;
			}
			ENDCG
		}
	}
}