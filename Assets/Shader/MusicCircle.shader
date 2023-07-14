Shader "Unlit/MusicCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        iChannel0("iChannel0", 2D) = "white" {}
        iChannel1("iChannel1", 2D) = "white" {}
        _ColorBG_A("ColorBG_A", Color) = (0.93, 0.71, 0.62, 1)
        _ColorBG_B("ColorBG_B", Color) = (0.9, 0.44, 0.44, 1)
        _Color_Star("Color_Star", Color) = (0.9, 0.91, 0.62, 1)
        _Color_Line("Color_Line", Color) = (0.9, 0.91, 0.82, 1)
        _Star_Power("StarPower", Range(0.1,2)) = 1
        _Line_Power("LinePower", Range(0.1,2)) = 1
        _Star_Size("StarSize", Range(0, 1)) = 1
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
            #define iResolution _ScreenParams 
            #define iTime _Time.y
            #define mix lerp
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 srcPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D iChannel0,iChannel1;
            fixed4 _ColorBG_A,_ColorBG_B,_Color_Star,_Color_Line;
            fixed _Star_Power,_Line_Power,_Star_Size;

            uniform float SOUND_MULTIPLIER;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = fixed4(v.vertex.x * -2, v.vertex.y * 2, 1, 1);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.srcPos = ComputeScreenPos(o.vertex); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.srcPos;
                uv -= fixed2(0.5, 0.5);
                uv.x *= iResolution.x/iResolution.y;

                // Calculate polar coordinates
                float r = length(uv);
                float a = atan2(uv.y, uv.x);
                   
                // Draw the lines
                half it = 2.0;
                float c = 0.0;
                for( float i = 0.0 ; i < it ; i += 1.0 )
                {
                    float i01 = i / it;
                    float rnd = tex2D( iChannel0, half2(i01, i01)).x;
                    float react = SOUND_MULTIPLIER * tex2D( iChannel1, half2(i01, 0.0) ).x * _Line_Power;    
                    
                    float c1 = (uv.x + 1.1 + react) * 0.004 * abs( 1.0 / sin( (uv.y +0.25) +
                                                                     sin(uv.x * 4.0 * rnd + rnd * 7.0 + iTime * 0.75) *
                                                                             (0.01 + 0.15*react)) );
                    c = clamp(c + c1, 0.0, 1.0);
                }
                
                float s = 0.0;
                float it2 = 20.0;
                for( float i = 0.0 ; i < it2 ; i += 1.0 )
                {
                    float i01 = i / it2;       
                    float react = SOUND_MULTIPLIER * tex2D( iChannel1, half2(i01, 0.0) ).x * _Star_Power;  
                    half2 rnd = tex2D( iChannel0, half2(i01, i01)).xy;
                    half2 rnd2 = rnd - 0.5;
                  
                    rnd2 = half2(0.85*sin(rnd2.x * 200.0 + rnd2.y * iTime * 0.1), 
                                -0.1 - 0.15 * sin(rnd2.x * rnd2.x * 200.0 + iTime  * rnd2.x * 0.25));
                    
                    float r1 = 1.0 - length(uv - rnd2);
                    float rad = ( 1.0 - clamp(0.03 * rnd.y + react * 0.05, 0.0, 1.0) );

                    r1 = smoothstep(rad, rad + 0.015, r1);
                    s += r1;
                }
                
                
                // Calculate the final color mixing lines and backgrounds
                half3 bg = mix( _ColorBG_A.rgb, _ColorBG_B.rgb, r);
                bg = mix(bg, _Color_Line.rgb, c);
                bg = mix(bg, _Color_Star.rgb, s);
                
                return fixed4(bg, 1.0);
            }

            ENDCG
        }
    }
}
