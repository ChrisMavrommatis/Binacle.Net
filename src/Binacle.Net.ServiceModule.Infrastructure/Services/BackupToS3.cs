using Amazon.S3;
using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.ServiceModule.Infrastructure.Services;

internal class BackupToS3 : BackgroundService
{
	private readonly IHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly ILogger<BackupToS3> logger;
	private readonly ConnectionString connectionString;

	public BackupToS3(IHostEnvironment environment,
		TimeProvider timeProvider,
		ILogger<BackupToS3> logger, 
		ConnectionString connectionString)
	{
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.logger = logger;
		this.connectionString = connectionString;
		
		this.connectionString
			.ThrowIfNotExists("ServiceUrl")
			.ThrowIfNotExists("AccessKeyId")
			.ThrowIfNotExists("SecretAccessKey")
			.ThrowIfNotExists("BucketName");
	}
    
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var internalMinutes = this.connectionString.GetOrDefault("InternalMinutes", 5);
		var initialDelaySeconds = this.connectionString.GetOrDefault("InitialDelaySeconds", 60);
		var dataDir = Path.Combine(
			this.environment.ContentRootPath, 
			this.connectionString.GetOrDefault("RootDir", "data")
		);

		this.logger.LogInformation("{Status} Backup To S3 Background Service. Watching {Directory}",
			"Starting",
			dataDir
		);
		
		// initial delay to allow other services to start
		await Task.Delay(TimeSpan.FromSeconds(initialDelaySeconds), stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await this.BackupAsync(dataDir, stoppingToken);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Error in Backup To S3 Background Service");
			}

			await Task.Delay(TimeSpan.FromMinutes(internalMinutes), stoppingToken);
		}
		
		this.logger.LogInformation("{Status} Backup To S3 Background Service. Watching {Directory}",
			"Stopping",
			dataDir
		);
	}

	private async Task BackupAsync(string rootDir, CancellationToken stoppingToken)
	{
		var uploadDelayMs = this.connectionString.GetOrDefault("UploadDelayMs", 100);
		var hashStoreFilePath = Path.Combine(
			rootDir, 
			this.connectionString.GetOrDefault("HashStore", ".hashstore.ndjson")
		);
		var store = new FileHashStore();
		await store.LoadAsync(hashStoreFilePath);
		var files = Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories);
		var client = this.CreateClient();
		foreach (var file in files)
		{
			if (file == hashStoreFilePath)
				continue; // skip hash store file
			try
			{
				var md5 = ComputeMD5(file);
				if (store.HashMatches(file, md5))
				{
					this.logger.LogDebug("Skipping {File}, no changes", file);
					continue; // no changes
				}

				// upload to S3
				var s3Key = file
					.Replace(rootDir, string.Empty)
					.TrimStart(Path.DirectorySeparatorChar);
				
				this.logger.LogDebug("Uploading {File} to S3 bucket as {Key}", file, s3Key);
				await client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
				{
					BucketName = this.connectionString.GetOrThrow("BucketName"),
					Key = s3Key,
					FilePath = file
				}, stoppingToken);
				this.logger.LogDebug("Uploaded {File} to S3 bucket as {Key}", file, s3Key);

				store.Set(file, md5);

				// wait a bit to avoid overwhelming the S3 server
				await Task.Delay(uploadDelayMs, stoppingToken);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Error uploading {File} to S3", file);
			}
		}

		await store.SaveAsync(hashStoreFilePath);
	}

	private AmazonS3Client CreateClient()
	{
		var config = new AmazonS3Config
		{
			ServiceURL = this.connectionString.GetOrThrow("ServiceUrl"),
			ForcePathStyle = this.connectionString.GetOrDefault("ForcePathStyle", true)
		};
		var client = new AmazonS3Client(
			this.connectionString.GetOrThrow("AccessKeyId"),
			this.connectionString.GetOrThrow("SecretAccessKey"),
			config
		);
		return client;
	}

	private static string ComputeMD5(string filePath)
	{
		using var md5 = System.Security.Cryptography.MD5.Create();
		using var stream = File.OpenRead(filePath);
		var hash = md5.ComputeHash(stream);
		return Convert.ToBase64String(hash);
	}
}
