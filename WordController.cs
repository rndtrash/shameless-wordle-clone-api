using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ShamelessWordleCloneAPI
{
    [ApiController]
    [Route("api")]
    public class WordController : Controller
    {
        [HttpGet("word")]
        public IActionResult GetWord(string? dictionary = "english", double? utcOffset = 0)
        {
            dictionary = dictionary ?? "english";
            utcOffset = utcOffset ?? 0;

            if (!DictionaryManager.Instance.Exists(dictionary))
                return BadRequest("dictionary doesn't exist");

            if (utcOffset < -12 * 60 || utcOffset > 14 * 60)
                return BadRequest("utc offset is too small/big");

            try
            {
                var dict = DictionaryManager.Instance.Get(dictionary);
                return Content(
                    dict.WordsShuffled[DateTimeOffset.UtcNow.AddMinutes(utcOffset.Value).ToUnixTimeSeconds() / 86400 % dict.Size]
                    );
            }
            catch (Exception)
            {
                return BadRequest("internal error");
            }
        }

        [HttpGet("dictionaries")]
        public IActionResult GetDictionaries()
        {
            try
            {
                return Content(string.Join('\n', DictionaryManager.Instance.GetAll()));
            }
            catch (Exception)
            {
                return BadRequest("internal error");
            }
        }

        [HttpGet("dictionary")]
        public IActionResult GetDictionary(string dictionary)
        {
            try
            {
                var dict = DictionaryManager.Instance.Get(dictionary);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // oh fuck off
                return Content(
                    $"{dict.Name}\n{dict.Description}\n{dict.Glyph}\n{dict.H}\n{dict.S}\n{dict.L}"
                    );
            }
            catch (Exception)
            {
                return BadRequest("internal error");
            }
        }
    }
}