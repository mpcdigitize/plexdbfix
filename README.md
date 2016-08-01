Important! Before you proceed always backup your original database file and stop Plex Server.

Why do you need to make this changes?

Plex Server database contains information about file paths of your media files. Therefore, when you move it form one location to another, Plex server will not be able to locate them unless you make actual changes. As you probably noticed, we are changing information for video files. We do not touch anything related to artwork. It’s not necessary to do it. Database holds only reference to these files. Therefore you have two options:

    Copy/Paste folder containing artwork
    Start from scratch. Create sections that match sections already created in existing database. Scan media folder and allow Plex download necessary metadata. Finally, substitute new created database with the saved one.

Where is Plex Server database located?

Depending on your platform and server setup, you should be able to navigate to Plex Media Server folder and find Plug-in Support. There is a Database folder containing a file that we are looking for. This is an example showing location on my home PC

“\Plex Media Server\Plex Media Server\Plug-in Support\Databases\com.plexapp.plugins.library.db”

Final step

Before we complete moving database to a new location, we have to do delete two files. Do not worry they will be recreated automatically when Plex Server starts again. If we fail to do so, Plex Server will simply not recognize our new database. These files are in the same folder as our database and they are:

com.plexapp.dlna.db-shm

com.plexapp.dlna.db-wal
