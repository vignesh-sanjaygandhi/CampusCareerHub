using Microsoft.AspNetCore.Mvc;
using CampusBackend.Data;
using CampusBackend.Models;
using System;
using System.Linq;

namespace CampusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CareerController : ControllerBase
    {
        private readonly CampusRepository _repository = new CampusRepository();

        // 1. Standard Endpoint: Get all jobs
        [HttpGet("jobs")]
        public IActionResult GetJobs()
        {
            return Ok(_repository.GetAllJobs());
        }

        // 2. AI-Style Feature: Rule-Based Resume Skill Matcher
        [HttpGet("match")]
        public IActionResult GetRecommendations([FromQuery] string studentSkills)
        {
            var allJobs = _repository.GetAllJobs();
            
            if (string.IsNullOrWhiteSpace(studentSkills)) 
                return Ok(allJobs);

            // Clean up the user's input skills
            var userSkills = studentSkills.ToLower().Split(',')
                                          .Select(s => s.Trim())
                                          .ToList();

            // Calculate match percentage for each job
            var matchedJobs = allJobs.Select(job => {
                var jobSkills = job.RequiredSkills.ToLower().Split(',')
                                                  .Select(s => s.Trim())
                                                  .ToList();
                
                int matchCount = jobSkills.Count(js => userSkills.Contains(js));
                double matchPercentage = jobSkills.Count > 0 
                    ? ((double)matchCount / jobSkills.Count) * 100 
                    : 0;

                return new {
                    JobDetails = job,
                    MatchPercentage = Math.Round(matchPercentage, 0)
                };
            })
            .OrderByDescending(x => x.MatchPercentage) // Best matches at the top
            .ToList();

            return Ok(matchedJobs);
        }
    }
}