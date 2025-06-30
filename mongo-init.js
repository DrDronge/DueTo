// MongoDB initialization script
// This script runs when the MongoDB container starts for the first time

// Switch to the admin database
db = db.getSiblingDB('admin');

// Create the dueto database
db = db.getSiblingDB('dueto');

// Create a user for the dueto database with proper permissions
db.createUser({
  user: 'admin',
  pwd: 'password',
  roles: [
    {
      role: 'readWrite',
      db: 'dueto'
    },
    {
      role: 'dbAdmin',
      db: 'dueto'
    }
  ]
});

// Create the Tasks collection
db.createCollection('Tasks');

print('MongoDB initialization completed successfully');