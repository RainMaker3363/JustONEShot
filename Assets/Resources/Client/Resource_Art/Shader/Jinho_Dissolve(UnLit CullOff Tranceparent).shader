Shader "Jinho/Dissolve(UnLit CullOff Tranceparent)" 
{
Properties 
{
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_ColorTint ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
	_Cutoff ("Alpha cutoff", Range (0,1)) = 0.5
/////////////////////////////디솔브 쉐이더
        _DissolveTex ("Dissolve Map", 2D) = "white" {}
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,1,1,0)
        _DissolveIntensity ("Dissolve Intensity", Range(0.0, 1.1)) = 0
        _DissolveEdgeRange ("Dissolve Edge Range", Range(0.0, 1.0)) = 0            
        _DissolveEdgeMultiplier ("Dissolve Edge Multiplier", Float) = 1	
/////////////////////////////////

}
SubShader 
{
	Tags { "RenderType" = "Transparent" }

//used for backface culling
	Cull Off

// Surface shaders are placed between CGPROGRAM and ENDCG
// - They use #pragma to let unity know its a surface shader
// - Must be in a SubShader block
CGPROGRAM
#pragma surface surf Unlit alphatest:_Cutoff
	struct Input 
	{
		float2 uv_MainTex;
         ///////////////////////////////디솔브 추가 설명
			float3 viewDir;
            float2 uv_DissolveTex;	
         /////////////////////////////////   	
	};
	sampler2D _MainTex;	

	// applies a color tint to the shader	
	fixed4 _ColorTint;

	half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
	{
		return half4(s.Albedo, s.Alpha);
	}
      //////////////////////////////////////////// 디솔브 쉐이더 추가
        sampler2D _DissolveTex;		
        
        uniform float4 _DissolveEdgeColor;      
        uniform float _DissolveEdgeRange;
        uniform float _DissolveIntensity;
        uniform float _DissolveEdgeMultiplier;     
      ////////////////////////////////////////////   
	// applies the texture to the UV's
	void surf (Input IN, inout SurfaceOutput o) 
	{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _ColorTint;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
     //////////////////////////////////////////////////////////////디솔브 쉐이더 추가.
            float4 dissolveColor = tex2D(_DissolveTex, IN.uv_DissolveTex);                  
            half dissolveClip = dissolveColor.r - _DissolveIntensity;
            half edgeRamp = max(0, _DissolveEdgeRange - dissolveClip);
            clip( dissolveClip ); 		
            
            float4 texColor = tex2D(_MainTex, IN.uv_MainTex);                
            o.Albedo = lerp( texColor, _DissolveEdgeColor, min(2, edgeRamp * _DissolveEdgeMultiplier) );
      //////////////////////////////////////////////////////
}
ENDCG
} 
}