Shader "Custom/WorldCutoffDual"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MinWorldX ("Min World X", Float) = -100.0
        _MaxWorldX ("Max World X", Float) = 100.0
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
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MinWorldX;
            float _MaxWorldX;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Cutoff Left: Discard if world X is LESS than Min
                clip(i.worldPos.x - _MinWorldX);
                
                // Cutoff Right: Discard if world X is GREATER than Max
                clip(_MaxWorldX - i.worldPos.x);

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}