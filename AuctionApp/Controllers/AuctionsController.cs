using AuctionApp.Data;
using AuctionApp.Data.Tables;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Nancy.Json;
using Newtonsoft.Json;
using System.Text.Json;

namespace AuctionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : Controller
    {
        private AuctionsDatabase _database = new();
       

        #region Примеры

        [HttpGet("[action]")]
        public string GetFirstAuctionNumber()
        {
            return _database.Auctions.First().Number; 
        }

        [HttpGet("[action]")]
        public IList<Company> GetCompanies()
        {
            return (from company in _database.Companies
                   where company.Phone.StartsWith("+79")
                   select company).ToList();
        }

        [HttpDelete("Action")]
        public void DeleteCompany()
        {
            using (var t = _database.BeginTransaction())
            {
                var companiesToDelete = _database.Companies.Take(2).ToList();

                _database.Delete(companiesToDelete[0]);
                _database.Delete(companiesToDelete[1]);

                t.Commit();
            }
        }
        
        #endregion

        #region Для задания

        [HttpPost("[action]")]
        public async Task<ActionResult> UploadJson()
        {
            string jsonText;
            var file = Request.Form.Files[0];

            using (var stream = file.OpenReadStream())
            using (var sr = new StreamReader(stream))
                jsonText = sr.ReadToEnd();

            //Заменить на обработку jsonText
  
            var s = JsonConvert.DeserializeObject<Auction>(jsonText);
 
            foreach (var lot in s.Lots)
            {
                _database.Insert(lot);
                foreach (var company in lot.Companies)
                {
                    _database.Insert(company);
                    var ls = new LotCompany()
                    {
                        Id = Guid.NewGuid(),
                        LotId = lot.Id,
                        CompanyId = company.Id
                    };
                    _database.Insert(ls);   
                }

            }

            return Ok();
        }

        [HttpGet("[action]/{search}")]
        public object LoadData(string search)
        {
            // заменить на запрос из задания
            return (from company in _database.Companies
                    join lotCompany in _database.LotCompanies on company.Id equals lotCompany.CompanyId
                    join companyOwnerships in _database.CompanyOwnerships on company.OwnershipId equals companyOwnerships.Id
                    join lot in _database.Lots on lotCompany.LotId equals lot.Id
                    join auction in _database.Auctions on lot.AuctionId equals auction.Id
                    where company.CompanyName.Contains(search)
                    select new
                    {
                        Number = auction.Number,
                        CompanyName = company.CompanyName,
                        Name = companyOwnerships.Name
                    }).ToList();
        }
        #endregion
    }
}
