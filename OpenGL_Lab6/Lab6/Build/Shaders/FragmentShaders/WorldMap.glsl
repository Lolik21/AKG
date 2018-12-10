#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube worldMap;

void main()
{    
    FragColor = texture(worldMap, TexCoords);
}