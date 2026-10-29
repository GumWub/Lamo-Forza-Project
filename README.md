I recommend that you check out the Talos folder it's mostly stateless (a glorified calculator)

btw, no ai was used in this project, I had to spend 90% of the time studying what I've implemented so far

This code is incomplete and still has plenty of critical bugs, placeholders, and a bunch of ownership problems (what owns what)
most of these problems are flagged by my comments in the code

plan: 

Talos will be written In C and be engine agnostic (rely on the game engine only for rendering)

- make my own wheel collider implementation (that would help a lot with my drivetrain/clutch model (the clutch slip will be easier to simulate))

- fix the ecu's bang bang idle control

- remodel the clutch

- refactor god classes (2 total)

- clean up the ugly architecture 

- impelent proper engine braking (pumping loss)

- work on a proper init sequence on and dependecy injection


that's all what I can remember, I will update the list when I'llget back to work
