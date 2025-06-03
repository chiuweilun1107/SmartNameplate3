using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.Entities;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardTextElementController : ControllerBase
    {
        private readonly ICardTextElementService _cardTextElementService;

        public CardTextElementController(ICardTextElementService cardTextElementService)
        {
            _cardTextElementService = cardTextElementService;
        }

        /// <summary>
        /// 取得指定卡片的文字標籤配置
        /// </summary>
        [HttpGet("{cardId}/side/{side}")]
        public async Task<ActionResult<List<CardTextElement>>> GetCardTextElements(int cardId, string side)
        {
            var elements = await _cardTextElementService.GetCardTextElementsAsync(cardId, side);
            return Ok(elements);
        }

        /// <summary>
        /// 取得指定卡片元素的標籤配置
        /// </summary>
        [HttpGet("{cardId}/side/{side}/element/{elementId}")]
        public async Task<ActionResult<CardTextElement>> GetCardTextElement(int cardId, string side, string elementId)
        {
            var element = await _cardTextElementService.GetCardTextElementAsync(cardId, side, elementId);
            if (element == null)
            {
                return NotFound();
            }
            return Ok(element);
        }

        /// <summary>
        /// 創建或更新文字標籤配置
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CardTextElement>> CreateCardTextElement([FromBody] CardTextElement element)
        {
            var created = await _cardTextElementService.CreateCardTextElementAsync(element);
            return CreatedAtAction(nameof(GetCardTextElement), 
                new { cardId = created.CardId, side = created.Side, elementId = created.ElementId }, 
                created);
        }

        /// <summary>
        /// 更新文字標籤配置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CardTextElement>> UpdateCardTextElement(int id, [FromBody] CardTextElement element)
        {
            var updated = await _cardTextElementService.UpdateCardTextElementAsync(id, element);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        /// <summary>
        /// 刪除文字標籤配置
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCardTextElement(int id)
        {
            var success = await _cardTextElementService.DeleteCardTextElementAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// 取得卡片實例資料
        /// </summary>
        [HttpGet("{cardId}/instance/{instanceName}")]
        public async Task<ActionResult<List<CardInstanceData>>> GetCardInstanceData(int cardId, string instanceName)
        {
            var data = await _cardTextElementService.GetCardInstanceDataAsync(cardId, instanceName);
            return Ok(data);
        }

        /// <summary>
        /// 儲存卡片實例資料
        /// </summary>
        [HttpPost("instance")]
        public async Task<ActionResult<CardInstanceData>> SaveCardInstanceData([FromBody] CardInstanceData instanceData)
        {
            var saved = await _cardTextElementService.SaveCardInstanceDataAsync(instanceData);
            return Ok(saved);
        }

        /// <summary>
        /// 刪除卡片實例
        /// </summary>
        [HttpDelete("{cardId}/instance/{instanceName}")]
        public async Task<ActionResult> DeleteCardInstance(int cardId, string instanceName)
        {
            var success = await _cardTextElementService.DeleteCardInstanceAsync(cardId, instanceName);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 