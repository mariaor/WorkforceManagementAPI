using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WorkforceManagementAPI.Models;

namespace WorkforceManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SkillsController : ApiController
    {
        private EmployeeManagementEntities db = new EmployeeManagementEntities();

        // GET: api/skills
        public IQueryable<Skills> GetSkills()
        {
            return db.Skills;
        }

        // GET: api/skills/5
        public IHttpActionResult GetSkills(int id)
        {
            Skills skills = db.Skills.Find(id);
            if (skills == null)
            {
                return NotFound();
            }

            return Ok(skills);
        }
 
        // POST: api/skills
        public IHttpActionResult PostSkills([FromBody]Skills skills)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Skills.Add(skills);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SkillsExists(skills.SkillID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // PUT: api/skills/5
        public IHttpActionResult PutSkills(int id, [FromBody]Skills skills)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != skills.SkillID)
            {
                return BadRequest();
            }

            db.Entry(skills).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/skills/5
        public IHttpActionResult DeleteSkills(int id)
        {
            Skills skills = db.Skills.Find(id);
            if (skills == null)
            {
                return NotFound();
            }

            db.Skills.Remove(skills);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SkillsExists(int id)
        {
            return db.Skills.Count(e => e.SkillID == id) > 0;
        }
    }
}