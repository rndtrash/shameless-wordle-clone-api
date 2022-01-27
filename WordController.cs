using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ShamelessWordleCloneAPI
{
    [ApiController]
    [Route("api/dictionary")]
    public class WordController : Controller
    {
        [HttpGet("{dictionary}/word")]
        public IActionResult GetWord(string dictionary)
        {
            if (!DictionaryManager.Instance.Exists(dictionary))
                return BadRequest("1\ndictionary doesn't exist");

            try
            {
                var dict = DictionaryManager.Instance.Get(dictionary);
                return Content(dict.GetWord());
            }
            catch (Exception)
            {
                return BadRequest("0\ninternal error");
            }
        }

        [HttpGet]
        public IActionResult GetDictionaries()
        {
            try
            {
                Console.WriteLine($"GetDictionaries {DictionaryManager.Instance.GetAll().Length}");
                return Content(string.Join('\n', DictionaryManager.Instance.GetAll()));
            }
            catch (Exception)
            {
                return BadRequest("0\ninternal error");
            }
        }

        [HttpGet("{dictionary}")]
        public IActionResult GetDictionary(string dictionary)
        {
            try
            {
                var dict = DictionaryManager.Instance.Get(dictionary);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // oh fuck off
                return Content(
                    $"{dict.Name}\n{dict.Description}\n{dict.Glyph}\n{dict.H}\n{dict.S}\n{dict.L}\n{dict.Size}"
                    );
            }
            catch (Exception)
            {
                return BadRequest("0\ninternal error");
            }
        }
    }
}