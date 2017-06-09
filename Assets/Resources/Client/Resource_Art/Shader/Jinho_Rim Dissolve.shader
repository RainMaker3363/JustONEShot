Shader "Jinho/RimDissolve" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0.0, 6.0)) = 3.0
        _DissolveTex ("Dissolve Map", 2D) = "white" {}
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,1,1,0)
        _DissolveIntensity ("Dissolve Intensity", Range(0.0, 1.1)) = 0
        _DissolveEdgeRange ("Dissolve Edge Range", Range(0.0, 1.0)) = 0            
        _DissolveEdgeMultiplier ("Dissolve Edge Multiplier", Float) = 1		
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert

		struct Input 
		{
			float4 color : COLOR;
			float2 uv_MainTex;
			float3 viewDir;
            float2 uv_DissolveTex;			
		};
		
		sampler2D _MainTex;
		float4 _RimColor;
		float _RimPower;
        sampler2D _DissolveTex;		
        
        uniform float4 _DissolveEdgeColor;      
        uniform float _DissolveEdgeRange;
        uniform float _DissolveIntensity;
        uniform float _DissolveEdgeMultiplier;        

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			
            float4 dissolveColor = tex2D(_DissolveTex, IN.uv_DissolveTex);                  
            half dissolveClip = dissolveColor.r - _DissolveIntensity;
            half edgeRamp = max(0, _DissolveEdgeRange - dissolveClip);
            clip( dissolveClip ); 		
            
            float4 texColor = tex2D(_MainTex, IN.uv_MainTex);                
            o.Albedo = lerp( texColor, _DissolveEdgeColor, min(2, edgeRamp * _DissolveEdgeMultiplier) );
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
