 Shader "Jinho/TransparentDiffuse Dissolve Unlit Cull Off" {
 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
/////////////////////////////디솔브 쉐이더
        _DissolveTex ("Dissolve Map", 2D) = "white" {}
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,1,1,0)
        _DissolveIntensity ("Dissolve Intensity", Range(0.0, 1.1)) = 0
        _DissolveEdgeRange ("Dissolve Edge Range", Range(0.0, 1.0)) = 0            
        _DissolveEdgeMultiplier ("Dissolve Edge Multiplier", Float) = 1	
/////////////////////////////////
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 200
//////////////////////////////메쉬 양면 렌더링
     Cull Off
////////////////////////////////


 CGPROGRAM
 #pragma surface surf NoLighting alpha

 
 sampler2D _MainTex;
 fixed4 _Color;

      fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
      {
         fixed4 c;
         c.rgb = s.Albedo; 
         c.a = s.Alpha;
         return c;
      }
 
         struct Input 
      {
         float2 uv_MainTex;
         ///////////////////////////////디솔브 추가 설명
			float3 viewDir;
            float2 uv_DissolveTex;	
         /////////////////////////////////   	
      };
      //////////////////////////////////////////// 디솔브 쉐이더 추가
        sampler2D _DissolveTex;		
        
        uniform float4 _DissolveEdgeColor;      
        uniform float _DissolveEdgeRange;
        uniform float _DissolveIntensity;
        uniform float _DissolveEdgeMultiplier;     
      ////////////////////////////////////////////   
 	 void surf (Input IN, inout SurfaceOutput o) 
 	 {
     fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
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
 
 Fallback "Transparent/VertexLit"
 }