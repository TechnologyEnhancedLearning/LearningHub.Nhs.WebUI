-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      10-06-2021
-- Purpose      Converts scorm formatted time string to seconds. Ported and adapted from
--				e-LfH to cater for packager and adapt time formats for scorm session time.
--
-- Modification History
-------------------------------------------------------------------------------
CREATE FUNCTION activity.ScormTimeToSeconds
(
	@TimeString nvarchar(20)
)

RETURNS Int

AS

BEGIN
	Declare @Hours	 nvarchar(10)
	Declare @Minutes nvarchar(10)
	Declare @Seconds nvarchar(10)
	Declare @intHours	int
	Declare @intMinutes int
	Declare @intSeconds int
	Declare @totalSeconds int

	Declare @Pos1 int
	Declare @Pos2 int
	Declare @Pos3 int

	Declare @PosEnd int

	IF @TimeString = ''
	BEGIN
		SET @totalSeconds = 0
	END
	Else
	BEGIN
		SET @PosEnd = charindex('.',@TimeString)
		IF @PosEnd > 0
		BEGIN
			SET @TimeString = SUBSTRING(@TimeString,1,@PosEnd-1)
		END

		Set @Pos1 = charindex(':',@TimeString,1)
		Set @Pos2 = charindex(':',@TimeString,@Pos1 + 1)
		Set @Pos3 = charindex('.',@TimeString,@Pos2 + 1)
		IF @Pos3 = 0
		BEGIN
			SET @Pos3 = len(@TimeString) + 1
		END

		Set @Hours = Substring(@TimeString, 1, @Pos1-1)
		Set @Minutes = Substring(@TimeString, @Pos1+1, @Pos2-@Pos1-1)
		Set @Seconds = Substring(@TimeString, @Pos2+1, @Pos3-@Pos2-1)
		Set @intHours = Convert(int,@Hours)
		Set @intMinutes = Convert(int,@Minutes)
		Set @intSeconds = Convert(int,@Seconds)

		Set @totalSeconds = (@intHours * 60 * 60) + (@intMinutes * 60) + @intSeconds
	END
		
	RETURN @totalSeconds
	
END
GO
