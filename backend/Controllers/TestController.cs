using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 測試資料表是否存在
        /// </summary>
        [HttpGet("tables")]
        public async Task<ActionResult<object>> TestTables()
        {
            try
            {
                // 檢查 CardTextElements 表
                var textElementsCount = await _context.CardTextElements.CountAsync();
                
                // 檢查 CardInstanceDatas 表
                var instanceDataCount = await _context.CardInstanceDatas.CountAsync();

                return Ok(new
                {
                    success = true,
                    message = "資料表存在",
                    cardTextElementsCount = textElementsCount,
                    cardInstanceDataCount = instanceDataCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "資料表檢查失敗",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// 測試創建一個標籤配置
        /// </summary>
        [HttpPost("create-test-tag")]
        public async Task<ActionResult<CardTextElement>> CreateTestTag()
        {
            try
            {
                // 檢查是否有卡片可以測試
                var firstCard = await _context.Cards.FirstOrDefaultAsync();
                if (firstCard == null)
                {
                    return BadRequest(new { message = "沒有卡片可以測試" });
                }

                var testElement = new CardTextElement
                {
                    CardId = firstCard.Id,
                    Side = "A",
                    ElementId = "test_element_" + DateTime.Now.Ticks,
                    TagType = "name",
                    TagLabel = "姓名",
                    DefaultContent = "測試用戶",
                    IsRequired = true,
                    SortOrder = 1
                };

                _context.CardTextElements.Add(testElement);
                await _context.SaveChangesAsync();

                return Ok(testElement);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "創建測試標籤失敗",
                    error = ex.Message
                });
            }
        }
    }
} 