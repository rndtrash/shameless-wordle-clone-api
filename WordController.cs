using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ShamelessWordleCloneAPI
{
    [ApiController]
    [Route("api/dictionary")]
    public class WordController : Controller
    {
        [HttpGet("{dictionary}/word")]
        public IActionResult GetWord(string dictionary, double? utcOffset = 0)
        {
            dictionary ??= "english";
            utcOffset ??= 0;

            if (!DictionaryManager.Instance.Exists(dictionary))
                return BadRequest("1\ndictionary doesn't exist");

            if (utcOffset < -12 * 60 || utcOffset > 14 * 60)
                return BadRequest("2\nutc offset is too small/big");

#if DEBUG
            Console.WriteLine($"Time with {utcOffset} minute offset: {DateTimeOffset.UtcNow.AddMinutes(utcOffset.Value)}; days since 1970: {(int)TimeSpan.FromSeconds(DateTimeOffset.UtcNow.AddMinutes(utcOffset.Value).ToUnixTimeSeconds()).TotalDays}");
#endif

            try
            {
                var dict = DictionaryManager.Instance.Get(dictionary);
                return Content(
                    dict.WordsShuffled[(int)TimeSpan.FromSeconds(DateTimeOffset.UtcNow.AddMinutes(utcOffset.Value).ToUnixTimeSeconds()).TotalDays % dict.Size]
                    );
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