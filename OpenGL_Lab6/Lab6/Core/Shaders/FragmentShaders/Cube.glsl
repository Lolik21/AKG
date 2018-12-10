#version 330 core
out vec4 FragColor;

in vec2 fragTexCoord;
uniform sampler2D texture1;

void main()
{
	vec2 MyTexture = vec2(fragTexCoord.x, -fragTexCoord.y);
	FragColor = texture(texture1, MyTexture);
}