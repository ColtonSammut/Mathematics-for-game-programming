Shader "Unlit/HeightShader"
{
    Properties
    {
        _SnowTex ("SnowTexture", 2D) = "white" {}
        _MounTex ("MountainTexture", 2D) = "white" {}
        _SandTex ("MountainTexture", 2D) = "white" {}
        _WateTex ("WaterTexture", 2D) = "white" {}
        _SnowHeight ("SnowHeight", Range(0.0, 1000)) = 50 
        _WaterHeight ("WaterHeight", Range(0.0, 1000)) = 0 
        _Height ("Height", Range(0.0, 1000)) = 10
        _MountainHeight ("MountainHeight", Range(0.0, 1000)) = 10
        _BlendHeight ("Blend Height", Range(0, 50)) = 5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float _SnowHeight;
            float _WaterHeight;
            float _Height;
            float _BlendHeight;
            float _MountainHeight;
            sampler2D snowTex;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float height : TEXCOORD1;
            };

            sampler2D _SnowTex;
            sampler2D _MounTex;
            sampler2D _SandTex;
            sampler2D _WateTex;
            float4 _SnowTex_ST;
            float4 _MounTex_ST;
            float4 _SandTex_ST;
            float4 _WateTex_ST;

            float heightblend(float height, float mid)
            {
                float t= (height - mid) / _BlendHeight;

                t = t + 0.5;

                if(t > 1)
                {
                    t = 1;
                }
                else if (t < 0)
                {
                    t = 0;
                }

                return t;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = TRANSFORM_TEX(v.uv, _SnowTex);
                o.uv = TRANSFORM_TEX(v.uv, _MounTex);
                o.uv = TRANSFORM_TEX(v.uv, _SandTex);
                o.uv = TRANSFORM_TEX(v.uv, _WateTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.height = v.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col;
                fixed4 snow = tex2D(_SnowTex, i.uv);
                fixed4 moun = tex2D(_MounTex, i.uv);
                fixed4 sand = tex2D(_SandTex, i.uv);
                fixed4 wate = tex2D(_WateTex, i.uv);
                
                if(i.height > _SnowHeight + _BlendHeight / 2)
                {
                    col = snow;
                }
                else if (i.height > _SnowHeight - _BlendHeight / 2)
                {
                    float t = heightblend(i.height , _SnowHeight);
                    col = (1 - t)*moun + t*snow;
                }
                else if(i.height > _MountainHeight + _BlendHeight / 2)
                {
                   col = moun;
                }
                else if(i.height > _MountainHeight - _BlendHeight / 2)
                {
                    float t = heightblend(i.height , _MountainHeight);
                    col = (1 - t)*sand + t*moun;
                }
                else if(i.height > _Height  + _BlendHeight / 2)
                {
                    col = sand;
                }
                else if(i.height > _WaterHeight)
                {
                   col = wate;
                }
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
