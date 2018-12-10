#version 330 core
out vec4 FragColor;

in vec3 fragColor;
in vec2 fragTexCoord;
uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	vec2 MyTexture = vec2(fragTexCoord.x, -fragTexCoord.y);
	FragColor = mix(texture(texture1, MyTexture), texture(texture2, MyTexture), 0.1f) * vec4(fragColor, 1.0);
}