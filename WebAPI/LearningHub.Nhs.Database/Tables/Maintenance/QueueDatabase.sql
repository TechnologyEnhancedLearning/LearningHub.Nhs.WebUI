CREATE TABLE [maintenance].[QueueDatabase](
  [QueueID] [int] NOT NULL,
  [DatabaseName] [sysname] NOT NULL,
  [DatabaseOrder] [int] NULL,
  [DatabaseStartTime] [datetime2](7) NULL,
  [DatabaseEndTime] [datetime2](7) NULL,
  [SessionID] [smallint] NULL,
  [RequestID] [int] NULL,
  [RequestStartTime] [datetime] NULL,
 CONSTRAINT [PK_QueueDatabase] PRIMARY KEY CLUSTERED
(
  [QueueID] ASC,
  [DatabaseName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
ALTER TABLE [maintenance].[QueueDatabase]  WITH CHECK ADD  CONSTRAINT [FK_QueueDatabase_Queue] FOREIGN KEY([QueueID])
REFERENCES [maintenance].[Queue] ([QueueID])
GO
ALTER TABLE [maintenance].[QueueDatabase] CHECK CONSTRAINT [FK_QueueDatabase_Queue]
GO