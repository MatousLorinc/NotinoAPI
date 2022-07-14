using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using NotinoAPI.Models;

namespace NotinoAPI.Modules
{
	public static class DocumentsModule
	{
		// název složky se zdrojovým dokumentem
		public const string SOURCE_DIR = "SourceFiles";
		// název složky s cílovým dokumentem
		public const string TARGET_DIR = "TargetFiles";
		// název zdrojového dokumentu
		public const string SOURCE_FILENAME = "Document1";
		// název cílového dokumentu
		public const string TARGET_FILENAME = "Document1";
		// přípona zdrojového dokumentu
		public const string SOURCE_EXTENSION = "xml";
		/// <summary>
		/// Vrátí zdrojový dokument nebo null
		/// </summary>
		/// <returns></returns>
		public static XDocument GetSourceFile()
		{
			string sourcePath = Path.Combine(Environment.CurrentDirectory, Path.Combine(SOURCE_DIR, SOURCE_FILENAME + "." + SOURCE_EXTENSION));
			if (File.Exists(sourcePath)) 
			{
				return XDocument.Load(sourcePath);
			}
			else
			{
				return null;
			}
		}
		/// <summary>
		/// Vytvoří zdrojový dokument
		/// </summary>
		/// <param name="file"></param>
		public static async void CreateSourceFile(IFormFile file)
		{
			using (var stream = File.Create(GetSourceFilePath()))
			{
				await file.CopyToAsync(stream);
			}
		}
		/// <summary>
		/// Vytvoří cílový dokument ze zdrojového dokumentu
		/// </summary>
		/// <param name="extension">přípona cílového dokumentu</param>
		public static async void CreateTargetFile(string extension = "json")
		{
			if (File.Exists(GetSourceFilePath()))
			{
				using (var sourceStream = File.Open(GetSourceFilePath(), FileMode.Open))
				{
					var reader = new StreamReader(sourceStream);
					string input = reader.ReadToEnd();
					var xdoc = XDocument.Parse(input);
					var doc = new Document
					{
						Title = xdoc.Root.Element("Title").Value,
						Text = xdoc.Root.Element("Text").Value
					};
					var serializedDoc = JsonConvert.SerializeObject(doc);
					await File.WriteAllTextAsync(GetTargetFilePath(extension), serializedDoc);
				}
			}
		}
		/// <summary>
		/// Vrátí cestu k cílovému dokumentu
		/// </summary>
		/// <param name="extension">přípona cílového dokumentu</param>
		/// <returns>Vrátí cestu k cílovému dokumentu</returns>
		public static string GetTargetFilePath(string extension)
		{
			return Path.Combine(Environment.CurrentDirectory, Path.Combine(TARGET_DIR, string.Format("{0}.{1}", TARGET_FILENAME, extension)));
		}
		/// <summary>
		/// Vrátí cestu k zdrojovému dokumentu
		/// </summary>
		/// <returns>Vrátí cestu k zdrojovému dokumentu</returns>
		public static string GetSourceFilePath()
		{
			return Path.Combine(Environment.CurrentDirectory, Path.Combine(SOURCE_DIR, string.Format("{0}.{1}", SOURCE_FILENAME, SOURCE_EXTENSION)));
		}
	}
}
