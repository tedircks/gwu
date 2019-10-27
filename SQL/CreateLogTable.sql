DROP TABLE LOG
CREATE TABLE LOG
(
LogId int IDENTITY PRIMARY KEY,
LogDate datetime DEFAULT CURRENT_TIMESTAMP,
LogLebel nvarchar(12) DEFAULT 'info',
LogMessage varchar(max)
)