CREATE TABLE [dbo].[TimeSignature](
	Id int IDENTITY(1,1) NOT NULL,
	Numerator int NOT NULL,
	Denominator int NOT NULL,
 CONSTRAINT [PK_KeySignature] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET IDENTITY_INSERT TimeSignature ON

insert into TimeSignature(Id, Numerator, Denominator)
values (1, 4, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (2, 3, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (3, 2, 2)

insert into TimeSignature(Id, Numerator, Denominator)
values (4, 6, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (5, 12, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (6, 7, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (7, 2, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (8, 5, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (9, 9, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (10, 3, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (11, 9, 16)

insert into TimeSignature(Id, Numerator, Denominator)
values (12, 6, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (13, 8, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (14, 12, 16)

insert into TimeSignature(Id, Numerator, Denominator)
values (15, 3, 2)

insert into TimeSignature(Id, Numerator, Denominator)
values (16, 6, 16)

insert into TimeSignature(Id, Numerator, Denominator)
values (17, 4, 2)

insert into TimeSignature(Id, Numerator, Denominator)
values (18, 4, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (19, 7, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (20, 13, 8)

insert into TimeSignature(Id, Numerator, Denominator)
values (21, 12, 4)

insert into TimeSignature(Id, Numerator, Denominator)
values (22, 8, 4)
SET IDENTITY_INSERT TimeSignature OFF

CREATE TABLE dbo.Style(
	Id int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(60) NULL,
 CONSTRAINT PK_Style PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET IDENTITY_INSERT Style ON

insert into Style(Id, [Name])
values (1, 'Classic')

insert into Style(Id, [Name])
values (2, 'Rock')

insert into Style(Id, [Name])
values (3, 'Jazz')

SET IDENTITY_INSERT Style OFF

CREATE TABLE dbo.Band(
	Id int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	StyleId int NOT NULL,
 CONSTRAINT PK_Band PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE dbo.Band  WITH CHECK ADD  CONSTRAINT FK_Band_Style FOREIGN KEY(StyleId)
REFERENCES dbo.Style (Id)
GO

ALTER TABLE dbo.Band CHECK CONSTRAINT FK_Band_Style
GO

CREATE TABLE dbo.Song(
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(500) NOT NULL,
	BandId int NULL,
	StyleId int NOT NULL,
	TicksPerQuarterNote int NOT NULL,
	TempoInBeatsPerMinute int NULL,
	TempoInMicrosecondsPerBeat int NULL,
	NumberBars int NULL,
	NumberOfTicks int NULL,
	DurationInSeconds int NULL,
	NumberTracks int NULL,
	TimeSignatureId int NULL,
	OriginalMidiBase64Encoded nvarchar(max) NOT NULL,
	NormalizedSongSerialized nvarchar(max) NULL,
	TotalEvents int NULL,
	TotalNoteEvents int NULL,
	TotalPitchBendEvents int NULL,
	TotalControlChangeEvents int NULL,
	TotalSustainPedalEvents int NULL,
	TotalChannels int NULL,
	TotalInstruments int NULL,
	TotalPercussionInstruments int NULL,
	TotalTempoChanges int NULL,
	TotalDifferentPitches int NULL,
	TotalUniquePitches int NULL,
	HighestPitch int NULL,
	LowestPitch int NULL,
	TotalChannelIndependentEvents int NULL,
	TotalProgramChangeEvents int NULL,
	TotalChunks int NULL,
	TotalMelodicChunks int NULL,
	TotalChordChunks int NULL,
	HasMoreThanOneChannelPerChunk bit NULL,
	HasMoreThanOneInstrumentPerChunk bit NULL,
	HasPercusion bit NULL
 CONSTRAINT PK_Song PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.Song  WITH CHECK ADD  CONSTRAINT FK_Song_Band FOREIGN KEY(BandId)
REFERENCES dbo.Band (Id)
GO

ALTER TABLE dbo.Song CHECK CONSTRAINT FK_Song_Band
GO

ALTER TABLE dbo.Song  WITH CHECK ADD  CONSTRAINT FK_Song_Style FOREIGN KEY(StyleId)
REFERENCES dbo.Style (Id)
GO

ALTER TABLE dbo.Song CHECK CONSTRAINT FK_Song_Style
GO

ALTER TABLE dbo.Song  WITH CHECK ADD  CONSTRAINT FK_Song_TimeSignature FOREIGN KEY(TimeSignatureId)
REFERENCES dbo.TimeSignature (Id)
GO

ALTER TABLE dbo.Song CHECK CONSTRAINT FK_Song_TimeSignature
GO


