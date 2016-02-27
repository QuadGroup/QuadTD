//#EditorFriendly
//#node13:posx=-409.5:posy=292:title=ParamFloat:title2=ParamFloat3:input0=1:input0type=float:
//#node12:posx=-272.5:posy=202:title=Multiply:input0=1:input0type=float:input0linkindexnode=11:input0linkindexoutput=0:input1=1:input1type=float:input1linkindexnode=13:input1linkindexoutput=0:
//#node11:posx=-402.5:posy=212:title=TexWithXform:title2=TexWithXform3:input0=(0,0):input0type=Vector2:
//#node10:posx=-147.5:posy=103:title=ParamFloat:title2=Glossiness:input0=1:input0type=float:
//#node9:posx=-453.5:posy=97:title=ParamFloat:title2=specpower:input0=1:input0type=float:
//#node8:posx=-444.5:posy=1:title=TexWithXform:title2=specular:input0=(0,0):input0type=Vector2:
//#node7:posx=-272.5:posy=34:title=Multiply:input0=1:input0type=float:input0linkindexnode=8:input0linkindexoutput=0:input1=1:input1type=float:input1linkindexnode=9:input1linkindexoutput=0:
//#node6:posx=-154.5:posy=191:title=UnpackNormal:input0=0:input0type=float:input0linkindexnode=12:input0linkindexoutput=0:
//#node5:posx=-254.5:posy=-119:title=TexWithXform:title2=maintexture:input0=(0,0):input0type=Vector2:
//#node4:posx=0:posy=0:title=Lighting:title2=On:
//#node3:posx=0:posy=0:title=DoubleSided:title2=Back:
//#node2:posx=0:posy=0:title=FallbackInfo:title2=Transparent/Cutout/VertexLit:input0=1:input0type=float:
//#node1:posx=0:posy=0:title=LODInfo:title2=LODInfo1:input0=600:input0type=float:
//#masterNode:posx=0:posy=0:title=Master Node:input0linkindexnode=5:input0linkindexoutput=0:input2linkindexnode=7:input2linkindexoutput=0:input3linkindexnode=10:input3linkindexoutput=0:input5linkindexnode=6:input5linkindexoutput=0:
//#sm=2.0
//#blending=Normal
//#ShaderName
Shader "ShaderFusion/jf specular bumped" {
	Properties {
_Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
//#ShaderProperties
_maintexture ("maintexture", 2D) = "white" {}
_specular ("specular", 2D) = "white" {}
_specpower ("specpower", Float) = 1
_Glossiness ("Glossiness", Float) = 1
_TexWithXform3 ("TexWithXform3", 2D) = "white" {}
_ParamFloat3 ("ParamFloat3", Float) = 1
	}
	Category {
		SubShader { 
//#Blend
ZWrite On
//#CatTags
Tags { "RenderType"="Opaque" }
Lighting On
Cull Back
//#LOD
LOD 600
//#GrabPass
		CGPROGRAM
//#LightingModelTag
#pragma surface surf ShaderFusion vertex:vert alphatest:_Cutoff
 //use custom lighting functions
 
 //custom surface output structure
 struct SurfaceShaderFusion {
	half3 Albedo;
	half3 Normal;
	half3 Emission;
	half Specular;
	half3 GlossColor; //Gloss is now three-channel
	half Alpha;
 };
 //forward lighting function
 inline half4 LightingShaderFusion (SurfaceShaderFusion s, half3 lightDir, half3 viewDir, half atten) {
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	viewDir = normalize(viewDir);
	half3 h = normalize (lightDir + viewDir);
	
	half diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0);
	
	half4 c;
	//Use gloss colour instead of gloss
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * s.GlossColor * spec) * (atten * 2);
	//We use gloss luminance to determine its overbright contribution
	c.a = s.Alpha + _LightColor0.a * Luminance(s.GlossColor) * spec * atten;
	return c;
 }
 //deferred lighting function
 inline half4 LightingShaderFusion_PrePass (SurfaceShaderFusion s, half4 light) {
	//Use gloss colour instead of gloss
	half3 spec = light.a * s.GlossColor;
	
	half4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * spec.rgb);
	//We use gloss luminance to determine its overbright contribution
	c.a = s.Alpha + Luminance(spec);
	return c;
 }
//#TargetSM
#pragma target 2.0
//#UnlitCGDefs
sampler2D _maintexture;
sampler2D _specular;
float _specpower;
float _Glossiness;
sampler2D _TexWithXform3;
float _ParamFloat3;
float4 _Color;
		struct Input {
//#UVDefs
float2 uv_maintexture;
float2 uv_specular;
float2 uv_TexWithXform3;
		INTERNAL_DATA
		};
		
		void vert (inout appdata_full v, out Input o) {
//#DeferredVertexBody
//#DeferredVertexEnd
		}
		void surf (Input IN, inout SurfaceShaderFusion o) {
			float4 normal = float4(0.0,0.0,1.0,0.0);
			float3 emissive = 0.0;
			float3 specular = 1.0;
			float gloss = 1.0;
			float3 diffuse = 1.0;
			float alpha = 1.0;
//#PreFragBody
float4 node5 = tex2D(_maintexture,IN.uv_maintexture.xy);
float4 node8 = tex2D(_specular,IN.uv_specular.xy);
float4 node11 = tex2D(_TexWithXform3,IN.uv_TexWithXform3.xy);
//#FragBody
normal = (float4(UnpackNormal(((node11) * (_ParamFloat3))),0));
gloss = (_Glossiness);
specular = ((node8) * (_specpower));
diffuse = (node5);
			
			o.Albedo = diffuse.rgb*_Color;
			#ifdef SHADER_API_OPENGL
			o.Albedo = max(float3(0,0,0),o.Albedo);
			#endif
			
			o.Emission = emissive*_Color;
			#ifdef SHADER_API_OPENGL
			o.Emission = max(float3(0,0,0),o.Emission);
			#endif
			
			o.GlossColor = specular*_SpecColor;
			#ifdef SHADER_API_OPENGL
			o.GlossColor = max(float3(0,0,0),o.GlossColor);
			#endif
			
			o.Alpha = alpha*_Color.a;
			#ifdef SHADER_API_OPENGL
			o.Alpha = max(float3(0,0,0),o.Alpha);
			#endif
			
			o.Specular = gloss;
			#ifdef SHADER_API_OPENGL
			o.Specular = max(float3(0,0,0),o.Specular);
			#endif
			
			o.Normal = normal;
//#FragEnd
		}
		ENDCG
		}
	}
//#Fallback
Fallback "Transparent/Cutout/VertexLit"
}
