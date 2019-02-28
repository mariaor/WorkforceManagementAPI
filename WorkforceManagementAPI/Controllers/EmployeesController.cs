using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WorkforceManagementAPI.Models;

namespace WorkforceManagement.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeesController : ApiController
    {
        private EmployeeManagementEntities db = new EmployeeManagementEntities();

        // GET: api/employees
        public IQueryable<Employees> GetEmployees()
        {
            return db.Employees;
        }

        // GET: api/employees/5
        public IHttpActionResult GetEmployees(int id)
        {
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return NotFound();
            }
            
            return Ok(employees);
        }

        // POST: api/employees
        public IHttpActionResult PostEmployees([FromBody]Employees employees)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employees.Add(employees);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EmployeesExists(employees.EmployeeID))
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

        // PUT: api/Employee/5
        public IHttpActionResult PutEmployees(int id, [FromBody]Employees employees)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employees.EmployeeID)
            {
                return BadRequest();
            }

            db.Entry(employees).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
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

        // DELETE: api/employees/5
        public IHttpActionResult DeleteEmployees(int id)
        {
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return NotFound();
            }

            //if we do not delete the skills first, when we try to delete the employee will cause error
            //because of the foreign key relation between the employee and the employee skills
            employees.Skills.Clear();
            db.Employees.Remove(employees);

            db.SaveChanges();
            
            return Ok();
        }

        // GET: api/employees/skills/?empid=1
        [Route("api/employees/skills")]
        public ICollection<Skills> GetSkills([FromUri]int empid)
        {
            Employees employees = db.Employees.Find(empid);
            ICollection<Skills> skills = (ICollection<Skills>)employees.Skills;

            return skills;
        }

        // GET: api/employees/skills/?empid=1&skillid=2
        [Route("api/employees/skills")]
        public ICollection<Skills> GetSkills([FromUri]int empid, [FromUri]int skillid)
        {
            Employees employees = db.Employees.Find(empid);

            ICollection<Skills> skills = employees.Skills;

            ICollection<Skills> skill = skills.Where(x => x.SkillID == skillid).ToList();

            return skill;
        }

        // DELETE: api/employees/deleteskills/?empid=1&skillid=2
        [Route("api/employees/deleteskills")]
        public IHttpActionResult DeleteSelectedSkills([FromUri]int empid, [FromUri]int skillid)
        {
            Employees employees = db.Employees.Find(empid);
            if (employees == null)
            {
                return NotFound();
            }

            Skills empskills = employees.Skills.FirstOrDefault(x => x.SkillID == skillid);

            if (empskills == null)
            {
                return NotFound();
            }

            employees.Skills.Remove(empskills);

            db.SaveChanges();

            return Ok();
        }

        // GET: api/employees/getemployeelackingskills/?empid=1
        [Route("api/employees/getemployeelackingskills")]
        public ICollection<Skills> GetEmployeeLackingSkills([FromUri]int empid)
        {
            Employees employees = db.Employees.Find(empid);
            ICollection<Skills> existingskills = (ICollection<Skills>)employees.Skills;

            ICollection<Skills> skills = db.Skills.ToList<Skills>();

            for (int i= skills.Count-1;i>=0;i--)
            {
                for (int j = 0; j < existingskills.Count; j++)
                {
                    if (skills.ElementAt(i).SkillID==existingskills.ElementAt(j).SkillID)
                    {
                        skills.Remove(skills.ElementAt(i));
                        break;
                    }
                }
            }

            return skills;
        }

        // POST: api/employees/postemployeeselectedskills/?empid=1
        [Route("api/employees/postemployeeselectedskills")]
        public IHttpActionResult PostEmployeeSelectedSkills([FromUri]int empid, IList<Int32> skills)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Employees employees = db.Employees.Find(empid);

            if (employees == null)
            {
                return NotFound();
            }
            
            foreach (Int32 skill in skills)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO EmployeeSkills (SkillsetEmployeeID, SkillsetSkillID) VALUES (@p0,@p1)", empid, skill);
            }
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

        private bool EmployeesExists(int id)
        {
            return db.Employees.Count(e => e.EmployeeID == id) > 0;
        }
    }
}