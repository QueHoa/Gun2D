Shader "2DxFX/AL/Shiny_Reflect" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_MainTex2 ("Base (RGB)", 2D) = "white" {}
		_Color ("_Color", Vector) = (1,1,1,1)
		_Distortion ("Distortion", Range(0, 1)) = 0
		_Value2 ("Value 2", Range(0, 1)) = 0
		_Value3 ("Value 3", Range(0, 1)) = 0
		_Value4 ("Value 4", Range(0, 1)) = 0
		_Alpha ("Alpha", Range(0, 1)) = 1
		[HideInInspector] _SrcBlend ("_SrcBlend", Float) = 0
		[HideInInspector] _DstBlend ("_DstBlend", Float) = 0
		[HideInInspector] _BlendOp ("_BlendOp", Float) = 0
		[HideInInspector] _Z ("_Z", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Transparent/VertexLit"
}