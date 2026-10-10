import React, { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [jobs, setJobs] = useState([]);
  const [skillsInput, setSkillsInput] = useState('');
  const [isMatchingMode, setIsMatchingMode] = useState(false);

  // This matches your C# Backend running on port 5237
  const API_BASE_URL = 'http://localhost:5237/api/career';

  useEffect(() => {
    fetchAllJobs();
  }, []);

  const fetchAllJobs = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/jobs`);
      const data = await response.json();
      setJobs(data);
      setIsMatchingMode(false);
      setSkillsInput('');
    } catch (error) {
      console.error("Error fetching jobs:", error);
    }
  };

  const handleAIRecommendation = async () => {
    if (!skillsInput.trim()) {
      fetchAllJobs();
      return;
    }
    
    try {
      const response = await fetch(`${API_BASE_URL}/match?studentSkills=${encodeURIComponent(skillsInput)}`);
      const data = await response.json();
      setJobs(data);
      setIsMatchingMode(true);
    } catch (error) {
      console.error("Error matching skills:", error);
    }
  };

  return (
    <div className="container">
      <header>
        <h1>CampusCareer Hub</h1>
        <p>Student Placement & Career Management Platform</p>
      </header>

      <div className="ai-matcher-card">
        <h2>🤖 Rule-Based Resume Matcher</h2>
        <p>Enter your technical skills separated by commas to see your best job matches.</p>
        <div className="input-group">
          <input 
            type="text" 
            placeholder="e.g. React, C#, SQL, HTML" 
            value={skillsInput}
            onChange={(e) => setSkillsInput(e.target.value)}
          />
          <button onClick={handleAIRecommendation}>Find Matches</button>
          {isMatchingMode && (
            <button className="reset" onClick={fetchAllJobs}>Clear</button>
          )}
        </div>
      </div>

      <h2>{isMatchingMode ? "Your Recommended Jobs" : "All Available Jobs"}</h2>
      
      <div className="jobs-grid" style={{ marginTop: '1.5rem' }}>
        {jobs.map((item, index) => {
          const job = isMatchingMode ? item.jobDetails : item;
          const matchScore = isMatchingMode ? item.matchPercentage : null;
          
          return (
            <div key={index} className="job-card">
              <div className="job-header">
                <div>
                  <h3>{job.title}</h3>
                  <div className="company">🏢 {job.company}</div>
                </div>
                {isMatchingMode && (
                  <span className="match-badge">{matchScore}% Match</span>
                )}
              </div>
              <div className="skills">
                <strong>Required Skills:</strong>
                <div style={{ marginTop: '0.5rem' }}>
                  {job.requiredSkills.split(',').map((skill, i) => (
                    <span key={i} className="skill-tag">{skill.trim()}</span>
                  ))}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

export default App;