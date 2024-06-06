import React, { useState, useEffect } from 'react';
import axios from 'axios';

const TrainingSessions = () => {
  const [sessions, setSessions] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10; // Adjust based on your needs

  //TODO AWS Amplify env variable isn't configured properly yet...
  const BASE_API_URL = process.env.BASE_API_URL || "https://api.gym.ryanriddiford.com";

  useEffect(() => {
    const fetchSessions = async () => {
      try {
        const res = await axios.get(`${BASE_API_URL}/api/trainingSession/page/${currentPage}`);
        setSessions(res.data);
      } catch (err) {
        console.error("Error fetching data: ", err);
      }
    };

    fetchSessions();
  }, [currentPage]);

  return (
    <div className="container mx-auto px-4">
      <div className="overflow-x-auto relative shadow-md sm:rounded-lg">
        <table className="w-full text-sm text-left text-gray-500 dark:text-gray-400">
          <thead className="text-xs text-gray-700 uppercase bg-gray-50 dark:bg-gray-700 dark:text-gray-400">
            <tr>
              <th scope="col" className="py-3 px-6">Workout Name</th>
              <th scope="col" className="py-3 px-6">Date</th>
              <th scope="col" className="py-3 px-6">Duration</th>
              <th scope="col" className="py-3 px-6">Details</th>
            </tr>
          </thead>
          <tbody>
            {sessions.map(session => (
              <tr key={session.id} className="bg-white border-b dark:bg-gray-800 dark:border-gray-700">
                <td className="py-4 px-6">{session.workoutName}</td>
                <td className="py-4 px-6">{new Date(session.date).toLocaleDateString()}</td>
                <td className="py-4 px-6">{session.workoutDuration}</td>
                <td className="py-4 px-6">
                  <button className="font-medium text-blue-600 dark:text-blue-500 hover:underline">
                    View Exercises
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="flex justify-between items-center mt-4">
        <button
          onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
          disabled={currentPage === 1}
          className="px-4 py-2 text-sm text-blue-700 bg-blue-100 rounded-lg hover:bg-blue-200 disabled:text-gray-500 disabled:bg-gray-300"
        >
          Previous
        </button>
        <button
          onClick={() => setCurrentPage((prev) => prev + 1)}
          className="px-4 py-2 text-sm text-blue-700 bg-blue-100 rounded-lg hover:bg-blue-200"
        >
          Next
        </button>
      </div>
    </div>
  );
};

export default TrainingSessions;
