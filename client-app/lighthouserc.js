module.exports = {
  ci: {
    collect: {
      startServerCommand: 'serve -s build',
      url: ['http://localhost:3000/'],
      startServerReadyPattern: 'Server is running on PORT 4000',
      startServerReadyTimeout: 20000, // milliseconds
      numberOfRuns: 5,
    },
    upload: {
      target: 'temporary-public-storage',
    },
  },
};