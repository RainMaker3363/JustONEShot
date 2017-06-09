Shader "Jinho/Color_Detail_Lit_Bump_Rim" {
     Properties {
         _MainTex ("Base (RGB)", 2D) = "white" {}
         _Detail ("Detail (RGB)", 2D) = "gray" {}
         _Lightmap ("LightMap (RGB)", 2D) = "gray" {}
         _BumpMap ("Bumpmap", 2D) = "bump" {}
         _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
           _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
         
         
     }
     SubShader {
         
         Pass{
                         
             Lighting On
             Fog { Mode Off }
             SetTexture [_MainTex] { combine texture * primary Double, texture * primary}
             //SetTexture [_Detail] { combine previous * texture Double, previous}
             
         }
         Tags { "RenderType" = "Opaque" }
       CGPROGRAM
       #pragma surface surf WrapLambert
       
       half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
           half NdotL = dot (s.Normal, lightDir);
           half diff = NdotL * 0.2 + 0.8;
           half4 c;
           c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
           c.a = s.Alpha;
           return c;
       }
 
       struct Input {
           float2 uv_MainTex;
           float2 uv_Detail;
           float2 uv_BumpMap;
           float2 uv_Lightmap;
           float3 viewDir;
       };
       sampler2D _MainTex;
       sampler2D _Detail;
       sampler2D _BumpMap;
       sampler2D _LightMap;
       float4 _RimColor;
       float _RimPower;
       
       sampler2D _Lightmap;
       void surf (Input IN, inout SurfaceOutput o) {
           o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
           o.Albedo *= tex2D (_Detail, IN.uv_Detail).rgb * 1;
           o.Albedo = ((o.Albedo) + (tex2D (_MainTex, IN.uv_MainTex).rgb))/2;
           o.Albedo *= tex2D (_Lightmap, IN.uv_Lightmap).rgb * 2;
           o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
           half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
           o.Emission = _RimColor.rgb * pow (rim, _RimPower);
           
       }
       ENDCG
     } 
     FallBack "VertexLit", 2
 }