﻿namespace Binacle.Net.Api.ServiceModule.Models;

internal record StatelessTokenGenerationRequest(string Email, string UserGroup);
internal record StatelessTokenGenerationResult(bool Success, string? Token = null, string? TokenType = null, int? ExpiresIn = null);
