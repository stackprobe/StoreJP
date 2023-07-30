CALL C:\Factory\SetEnv.bat
CALL ff
CALL Clean

CALL db
CALL qq

CALL dl
CALL qq

CALL dd
CALL qq

MD C:\home\GitHub\StoreJ\Dev
MD C:\home\GitHub\StoreJ\DevBin
MD C:\home\GitHub\StoreJ\DevLabo
MD C:\home\GitHub\StoreJ\Factory
C:\Factory\Petra\makeMITLicense.exe C:\home\GitHub\StoreJ\LICENSE

C:\Factory\Petra\GitRelease.exe J
C:\apps\CopyDevDevBinToStoreP\CopyDevDevBinToStoreP.exe Dev J
C:\apps\CopyDevDevBinToStoreP\CopyDevDevBinToStoreP.exe DevBin J
C:\apps\CopyDevDevBinToStoreP\CopyDevDevBinToStoreP.exe DevLabo J
C:\apps\GitHubRepositoryFilter\GitHubRepositoryFilter.exe C:\home\GitHub\StoreJ
C:\apps\GitCommit\GitCommit.exe v1s C:\home\GitHub\StoreJ

TIMEOUT 2
