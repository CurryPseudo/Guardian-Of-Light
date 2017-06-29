// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "2D/Fov Mesh"
 {  
     Properties
     {
     }
     SubShader
     {
         Tags 
         { 
             "RenderType" = "Opaque" 
             "Queue" = "Transparent+1" 
         }
 
         Pass
         {
             ZWrite Off
             Blend SrcColor One
             Cull Off
  
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile DUMMY PIXELSNAP_ON
             #include "UnityCG.cginc"
  
             sampler2D _MainTex;
             float4 _Color;
			 float _LightFactor;
 
             struct appdata_t
			 {
				float4 vertex   : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};
  
             v2f vert(appdata_t v)
             {
                 v2f o;
     
                 o.vertex = UnityObjectToClipPos(v.vertex);
     
                 return o;
             }
                                                     
             float4 frag(v2f IN) : COLOR
             {
				 float4 c = float4(1,1,1,1);
                 return c;
             }
 
             ENDCG
         }
     }
 }
