using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Notino.Homework
{
	public class Document
	{
		public string Title { get; set; }
		public string Text { get; set; }
	}

	class Program
	{
		static void Main2(string[] args)
		{

			// navrtdo napsaná lomítka mohou způsobit problémy na různých prostředích
			// proto je lepší použít více vnořených Path.Combine
			// magické řetězce bych vložil jako konstanty na začátek třídy
			var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Source Files\\Document1.xml");
			var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Target Files\\Document1.json");
			try
			{
				FileStream sourceStream = File.Open(sourceFileName, FileMode.Open);
				var reader = new StreamReader(sourceStream);
				string input = reader.ReadToEnd();
			}
			// odchytávání vyjímek obecné není nejvhodnější, lepší by bylo nejprve zvalidovat vstup a pak postupovat bez try catch
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			// metoda XDocument.Load dělá to samá ale zohledňuje popis enkódování
			var xdoc = XDocument.Parse("input");
			var doc = new Document
			{
				Title = xdoc.Root.Element("title").Value,
				Text = xdoc.Root.Element("text").Value
			};
			var serializedDoc = JsonConvert.SerializeObject(doc);
			// přijde mi jednodušší použít metodu File.WriteAllText(Async)
			var targetStream = File.Open(targetFileName, FileMode.Create, FileAccess.Write);
			var sw = new StreamWriter(targetStream);
			sw.Write(serializedDoc);
		}
	}
}