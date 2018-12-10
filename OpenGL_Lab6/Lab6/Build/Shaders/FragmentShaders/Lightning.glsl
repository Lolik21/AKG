#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragmentPosition;

uniform vec3 cameraPosition;
uniform vec3 lightPos;
uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
	vec3 norm = normalize(Normal);
	vec3 lightDirection = normalize(lightPos - FragmentPosition); 
	vec3 diffuse = max(dot(norm, lightDirection), 0.0) * lightColor;

	float specularStrength = 0.5;
	vec3 cameraDirection = normalize(cameraPosition - FragmentPosition);
	vec3 reflictedVector = reflect(-lightDirection, norm);
	float spec = pow(max(dot(cameraDirection, reflictedVector), 0.0), 32);
	vec3 specular = specularStrength * spec * lightColor;

	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * lightColor;

	vec3 result = (ambient + diffuse + specular) * objectColor;
	FragColor = vec4(result, 1.0);
}