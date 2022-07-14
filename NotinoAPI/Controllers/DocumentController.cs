using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotinoAPI.Models;
using NotinoAPI.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NotinoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		/// <summary>
		/// Vrátí modelu pro aktuální nahraný XML dokument, pokud existuje
		/// </summary>
		/// <returns>Document model</returns>
		[HttpGet]
		[ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetDocument()
		{
			// na
			var sourceFile = DocumentsModule.GetSourceFile();
			if(sourceFile != null)
			{
				var document = new Document() 
				{ 
					Title = sourceFile.Root.Element("Title").Value, 
					Text = sourceFile.Root.Element("Text").Value
				};
				return Ok(document);
			}
			else
			{
				return NotFound();
			}
		}

		/// <summary>
		/// Vytvoří ukázku modelu z url parametrů
		/// </summary>
		/// <param name="title">Titul dokumentu</param>
		/// <param name="text">Obsah dokumentu</param>
		/// <returns></returns>
		[HttpGet("SimulateDocument")]
		[ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult SimulateDocument(string title, string text)
		{
			var document = new Document()
			{
				Title = title,
				Text = text
			};
			return Ok(document);
		}

		/// <summary>
		/// Nahraje XML dokument a vytvoří jeho JSON kopii
		/// </summary>
		/// <param name="file">Uživatelem zvolený dokument</param>
		/// <returns></returns>
		[HttpPost("UploadDocument")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UploadDocument(IFormFile file)
		{
			string selectedFilePath = Path.GetTempFileName();
			// zde by měla být dodatečná validace (jestli se jedná o validní XML soubor, max velikost souboru)
			if (file.Length > 0)
			{
				DocumentsModule.CreateSourceFile(file);
				DocumentsModule.CreateTargetFile();
				return Ok(new { result = "File successfully uploaded." });
			}
			else
			{
				return BadRequest(new { result = "Invalid file, this file cant be uploaded." });
			}
			
		}

		/// <summary>
		/// Stáhne dokument pokud existuje
		/// </summary>
		/// <param name="extension">přípona souboru</param>
		/// <returns></returns>
		[HttpGet("DownloadDocument")]
		[ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Download(string extension)
		{
			var path = DocumentsModule.GetTargetFilePath(extension);
			if (System.IO.File.Exists(path))
			{
				var stream = System.IO.File.OpenRead(path);
				string fileName = string.Format("{0}.{1}", DocumentsModule.TARGET_FILENAME, extension);
				return File(stream, "application/octet-stream", fileName);
			}
			else
			{
				return NotFound();
			}
		}

		/// <summary>
		/// Odešle dokument jako příloha v emailu
		/// </summary>
		/// <param name="recipient">adresát</param>
		/// <returns></returns>
		[HttpGet("SendDocument")]
		[ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
		public IActionResult SendDocument(string recipient)
		{
			// připojení na poštovní server

			// vytvoření emailu
			// nastavení adresáta, odesílatele, hlavičky, těla
			// přidání souboru jako přílohy

			return Ok(new { result = "Email was successfully send" });
		}

	}
}
