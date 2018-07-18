import { trigger, state, animate, transition, style } from "@angular/animations";

export const fadeInAnimation =
    trigger("fadeInAnimation", [
        // route 'enter' transition
        transition(":enter", [

            // styles at start of transition
            style({ opacity: 0 }),

            // animation and styles at end of transition
            animate(".5s", style({ opacity: 1 }))
        ]),
    ]);
