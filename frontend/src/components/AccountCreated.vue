<template class="grey lighten-4">
<transition name="fade" mode="out-in"> 
  <div class="d-flex flex-column justify-center">
    <v-icon size="130" color="success">check_circle</v-icon>
		<div style="margin-top: 80px">
			<p class="text-center display-1">Criamos sua conta :)</p>
			<p class="text-center">O número da sua conta é <strong>{{accountNumber}}</strong>, faça seu <router-link to="/">login</router-link> para acessar as funcionalidades disponíveis.</p>
		</div>
	</div>
</transition>
</template>

<script>
export default {
  props: ["accountNumber"]
};
</script>
<style scoped>
p {
	color: #806e6e
}
a {
	text-decoration: none;
	
}
</style>