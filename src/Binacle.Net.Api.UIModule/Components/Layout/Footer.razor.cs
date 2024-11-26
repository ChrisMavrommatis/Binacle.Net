using Binacle.Net.Api.UIModule.Models;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Layout;

public partial class Footer : ComponentBase
{
	public List<Badge> Badges { get; set; } =
	[
		new Badge("GitHub Repo stars", Binacle.Net.Api.Metadata.GitHub, "https://img.shields.io/github/stars/chrismavrommatis/binacle.net?logoSize=auto"),
		new Badge("Binacle.Net Version", $"https://img.shields.io/badge/Current_Version-v{Binacle.Net.Api.Metadata.Version}-ff9100?labelColor=448aff"),
		new Badge("Docker Image Version", Binacle.Net.Api.Metadata.Dockerhub, "https://img.shields.io/docker/v/binacle/binacle-net?sort=semver&logo=docker&logoSize=auto&labelColor=white&color=blue"),
		new Badge("dotNet Version", $"https://img.shields.io/badge/_-{Binacle.Net.Api.Metadata.DotNetVersion}-512BD4?logo=dotnet&logoSize=auto&labelColor=512BD4"),
		new Badge("Swagger UI", "/swagger", "https://img.shields.io/badge/Visit_Swagger_UI-85EA2D?logo=swagger&logoSize=auto&labelColor=black"),
		new Badge("Postman Collection", Binacle.Net.Api.Metadata.Postman, "https://img.shields.io/badge/Get_Postman_Collection-FF6C37?logo=postman&logoSize=auto&labelColor=wh")
	];
}



