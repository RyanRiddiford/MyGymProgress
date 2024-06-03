import React, { useState, useEffect } from 'react';
import axios from 'axios';

function TrainingSessionsTable() {
  const [sessions, setSessions] = useState([]);
  const [page, setPage] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    fetchTrainingSessions(page, pageSize);
  }, [page]);

  const fetchTrainingSessions = async (page, pageSize) => {
    try {
      const response = await axios.get(`http://yourapi.com/trainingsessions?pageNumber=${page}&pageSize=${pageSize}`);
      setSessions(response.data);
    } catch (error) {
      console.error('Failed to fetch training sessions:', error);
    }
  };

  return (
    <div>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Title</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {sessions.map(session => (
            <tr key={session.id}>
              <td>{session.id}</td>
              <td>{session.title}</td>
              <td>{new Date(session.date).toLocaleDateString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <div>
        <button onClick={() => setPage(prev => prev - 1)} disabled={page === 1}>Previous</button>
        <button onClick={() => setPage(prev => prev + 1)}>Next</button>
      </div>
    </div>
  );
}

export default TrainingSessionsTable;
