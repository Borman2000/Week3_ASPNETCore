-- Create the new user with a specific password, allowing connections from any host ('%') or localhost only
CREATE USER 'exporter'@'%' IDENTIFIED BY 'exporter' WITH MAX_USER_CONNECTIONS 3;

-- Grant specific privileges (e.g., SELECT, INSERT) on a specific database ('my_database')
# GRANT SELECT, INSERT ON my_database.* TO 'exporter'@'%';
GRANT PROCESS, REPLICATION CLIENT, SELECT ON *.* TO 'exporter'@'%';

-- Grant all privileges on a specific database
-- GRANT ALL PRIVILEGES ON my_database.* TO 'exporter'@'%';

-- Apply the changes
FLUSH PRIVILEGES;
