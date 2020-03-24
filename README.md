# Must
The library for realize repeatable action in C#

Excaples : 

---------------------------------------------------------------

int errorSleepMs = 100;
CancellationTokenSource source = new CancellationTokenSource();
var must = new Must(errorSleepMs, source.Token);
var task = must.Exec(()=>{
  ........
});

----------------------------------------------------------------

int attempts = 10
int errorSleepMs = 100;
CancellationTokenSource source = new CancellationTokenSource();
var must = new Must(errorSleepMs, source.Token);

try{
  var task = await must.ExecAttempts(()=>{
  ........
  return 1;
  }, attempts);
}
catch(TaskCanceledException e){
  .................
}catch(AttemptsOverException){
  
}
