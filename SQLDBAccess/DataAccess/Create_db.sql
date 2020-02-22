DROP TABLE IF EXISTS  Occurrence
DROP TABLE IF EXISTS  Pattern
DROP TABLE IF EXISTS  PatternType
DROP TABLE IF EXISTS  PitchBendItem
DROP TABLE IF EXISTS  Note
DROP TABLE IF EXISTS  PatternTypes
DROP TABLE IF EXISTS  ArpeggioOccurrence
DROP TABLE IF EXISTS  Arpeggio
DROP TABLE IF EXISTS  MelodyPatternOccurrence
DROP TABLE IF EXISTS  MelodyPattern
DROP TABLE IF EXISTS  PitchPattern
DROP TABLE IF EXISTS  RythmPattern
DROP TABLE IF EXISTS  SongVersion
DROP TABLE IF EXISTS  TempoChange
DROP TABLE IF EXISTS  Bar
DROP TABLE IF EXISTS  Song
DROP TABLE IF EXISTS  Band
DROP TABLE IF EXISTS  Style
DROP TABLE IF EXISTS  TimeSignature

CREATE TABLE TimeSignature(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	Numerator int NOT NULL,
	Denominator int NOT NULL,
)


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

CREATE TABLE Style(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	[Name] nvarchar(60) NULL,
)
ALTER TABLE Style ADD CONSTRAINT UC_Style UNIQUE (Name);

SET IDENTITY_INSERT Style ON

insert into Style(Id, [Name])
values (1, 'Classic')

insert into Style(Id, [Name])
values (2, 'Rock')

insert into Style(Id, [Name])
values (3, 'Jazz')

SET IDENTITY_INSERT Style OFF

CREATE TABLE Band(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	StyleId bigint NOT NULL,
)
ALTER TABLE Band ADD CONSTRAINT UC_Band UNIQUE (Name);

ALTER TABLE Band  WITH CHECK ADD  CONSTRAINT FK_Band_Style FOREIGN KEY(StyleId)
REFERENCES Style (Id)
GO

ALTER TABLE Band CHECK CONSTRAINT FK_Band_Style
GO

CREATE TABLE Song(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	Name nvarchar(500) NOT NULL,
	BandId bigint NULL,
	StyleId bigint NOT NULL,
	TempoInBeatsPerMinute int NULL,
	TempoInMicrosecondsPerBeat int NULL,
	NumberBars int NULL,
	NumberOfTicks int NULL,
	DurationInSeconds int NULL,
	NumberTracks int NULL,
	TimeSignatureId bigint NULL,
	OriginalMidiBase64Encoded nvarchar(max) NOT NULL,
	ProcessedMidiBase64Encoded nvarchar(max) NULL,
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
)

ALTER TABLE Song  WITH CHECK ADD  CONSTRAINT FK_Song_Band FOREIGN KEY(BandId)
REFERENCES Band (Id)
GO

ALTER TABLE Song CHECK CONSTRAINT FK_Song_Band
GO

ALTER TABLE Song  WITH CHECK ADD  CONSTRAINT FK_Song_Style FOREIGN KEY(StyleId)
REFERENCES Style (Id)
GO

ALTER TABLE Song CHECK CONSTRAINT FK_Song_Style
GO

ALTER TABLE Song  WITH CHECK ADD  CONSTRAINT FK_Song_TimeSignature FOREIGN KEY(TimeSignatureId)
REFERENCES TimeSignature (Id)
GO

ALTER TABLE Song CHECK CONSTRAINT FK_Song_TimeSignature
GO

CREATE TABLE SongVersion(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	VersionNumber int not null,
	SongId bigint not null
) 


CREATE TABLE Note(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	Pitch tinyint NOT NULL,
	Volume tinyint NOT NULL,
	StartSinceBeginningOfSongInTicks bigint NOT NULL,
	EndSinceBeginningOfSongInTicks bigint NOT NULL,
	Instrument tinyint NOT NULL,
	IsPercussion bit null,
	SongVersionId bigint not null
)
ALTER TABLE Note  WITH CHECK ADD  CONSTRAINT FK_Note_SongVersionId FOREIGN KEY(SongVersionId)
REFERENCES SongVersion (Id)

CREATE TABLE Bar(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	BarNumber int NULL,
	TicksFromBeginningOfSong bigint NULL,
	TimeSignatureId bigint NULL,
	HasTriplets bit NULL,
	TempoInMicrosecondsPerQuarterNote int null,
	SongId bigint not null
)
ALTER TABLE Bar  WITH CHECK ADD  CONSTRAINT FK_Bar_TimeSignature FOREIGN KEY(TimeSignatureId)
REFERENCES TimeSignature (Id)

ALTER TABLE Bar  WITH CHECK ADD  CONSTRAINT FK_Bar_SongId FOREIGN KEY(SongId)
REFERENCES Song (Id)

CREATE TABLE PitchBendItem(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	TicksSinceBeginningOfSong bigint NULL,
	Pitch int NULL,
	NoteId bigint not null
) 
ALTER TABLE PitchBendItem  WITH CHECK ADD  CONSTRAINT FK_PitchBendItem_NoteId FOREIGN KEY(NoteId)
REFERENCES Note (Id)



CREATE TABLE TempoChange(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	TicksSinceBeginningOfSong bigint NULL,
	MicrosecondsPerQuarterNote int NULL,
	SongId bigint not null
) 
ALTER TABLE TempoChange  WITH CHECK ADD  CONSTRAINT FK_TempoChange_SongId FOREIGN KEY(SongId)
REFERENCES Song (Id)

create table PatternType(
	Id tinyint primary key clustered NOT NULL,
	TypeName varchar(10) not null
)

insert into PatternType(Id, TypeName)
values (1, 'Pitch')
insert into PatternType(Id, TypeName)
values (2, 'Rythm')
insert into PatternType(Id, TypeName)
values (3, 'Melody')
     

CREATE TABLE Pattern(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	AsString varchar(600) not null,
	PatternTypeId tinyint not null,
	CONSTRAINT IX_UniquePatterns UNIQUE (AsString, PatternTypeId)
) 
ALTER TABLE Pattern  WITH CHECK ADD  CONSTRAINT FK_Pattern_PatternType FOREIGN KEY(PatternTypeId)
REFERENCES PatternType (Id)

CREATE TABLE Occurrence(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	SongVersionId bigint not null,
	PatternId bigint not null,
	NoteId bigint not null
) 
ALTER TABLE Occurrence  WITH CHECK ADD  CONSTRAINT FK_Occurrence_SongVersionId FOREIGN KEY(SongVersionId)
REFERENCES SongVersion (Id)
ALTER TABLE Occurrence  WITH CHECK ADD  CONSTRAINT FK_Occurrence_NoteId FOREIGN KEY(NoteId)
REFERENCES Note (Id)
ALTER TABLE Occurrence  WITH CHECK ADD  CONSTRAINT FK_Occurrence_Pattern FOREIGN KEY(PatternId)
REFERENCES Pattern (Id)


