<template>
  <div class="restaurants">
    <h3 class="mt-3 mb-3"> Restaurants page </h3>
    <div>
        <div class="card m-2 p-2" v-for="r in restaurants" :key="r.id">
            <h5 class="card-title"> {{ r.name }} </h5>
            <p class="card-text"> Location: {{ r.city }}, {{ r.street }} {{r.houseNumber}} </p>
        </div>
    </div>
  </div>
</template>

<script>

import AccountService from "../services/account.service.js"

export default {
  name: 'Restaurants',
  data() {
    return {
      restaurants: []
    };
  },
  mounted() {
      AccountService.getRestaurants().then(response => {
          this.restaurants = response.data
      });
  },
};
</script>