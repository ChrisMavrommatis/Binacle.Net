﻿using Binacle.Net.Api.Users.Data.Schemas;

namespace Binacle.Net.Api.Users.Data.Entities;

internal class UserEntity : TableEntity
{
	public UserEntity(string email, string group)
	{
		this.RowKey = email;
		this.PartitionKey = group;
		this.Group = group;
	}
	public UserEntity()
	{

	}
	public string Email { get => RowKey; }
	public string Group { get; set; }
	public string HashedPassword { get; set; }
	public string Salt { get; set; }
}
